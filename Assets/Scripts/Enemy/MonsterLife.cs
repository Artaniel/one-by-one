using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

using Game.Events;

public class MonsterLife : MonoBehaviour
{
    [SerializeField] public float maxHP = 1;
    [HideInInspector] public float HP = 1;

    [SerializeField] protected GameObject absorbPrefab = null;
    [SerializeField] private GameObject enemyExplosionPrefab = null;
    [SerializeField] private float fadeInTime = 0.5f;
    [SerializeField] private bool autoChooseName = true;
    [SerializeField] private bool hitPlayerOnContact = true;

    [SerializeField] private EvilDictionary evilDictionary = null;

    // Apply listeners on start!!
    public static UnityEvent OnEnemyDead = new UnityEvent();
    public UnityEvent OnThisHit = new UnityEvent();
    public static MonsterDamagedEvent monsterDamaged = new MonsterDamagedEvent();

    [HideInInspector] public MonsterManager monsterManager = null;

    [SerializeField] private float timeKillToDestroyGObject = 0.5f;
    [SerializeField] private float timeKillToHideGObject = 0.15f;

    [SerializeField] private AudioClip hitSound = null;
    [SerializeField] private AudioClip[] hitSounds = null;
    [SerializeField] private float pauseBetweenConsecutiveSounds = 1f;
    private float lastHitSoundTime = 0;

    protected virtual bool ReceiveFullDamage()
    {
        return isBoy();
    }

    public class MonsterDamagedEvent : UnityEvent<float, GameObject> { }

    private void Awake()
    {
        HP = maxHP;
        
        sprites = GetComponentsInChildren<SpriteRenderer>();
        monsterName = GetComponentInChildren<TMPro.TextMeshPro>();
        audioSource = GetComponent<AudioSource>();
        aiAgent = GetComponent<AIAgent>();

        ChooseMyName();
    }

    protected virtual void Start()
    {
        FadeIn(fadeInTime);
        sprites = GetComponentsInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if (Pause.Paused) return;
        if (fadeInLeft != 0) FadeInLogic();
        CustomUpdate();
    }

    protected virtual void CustomUpdate() { }

    private void FadeInLogic()
    {
        fadeInLeft = Mathf.Max(fadeInLeft - Time.deltaTime, 0);

        UpdateFadeColor();
    }

    protected virtual bool VulnerableCondition(GameObject source)
    {
        return true;
    }

    private void _FullHitEffect()
    {
        if (hitSound && Time.time - lastHitSoundTime > pauseBetweenConsecutiveSounds)
        {
            lastHitSoundTime = Time.time;
            if (hitSounds.Length > 0)
            {
                var sound = hitSounds[Random.Range(0, hitSounds.Length)];
                AudioManager.Play(sound, audioSource);
            }
            AudioManager.Play(hitSound, audioSource);
        }
        FullHitEffect();
    }

    public enum DamageType { None, Damaged, Absorb };

    protected virtual void FullHitEffect() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source">Object that causes damage. Can be null</param>
    /// <param name="damage">Damage value</param>
    /// <param name="ignoreInvulurability">Should ignore invulnerability</param>
    /// <param name="ignoreSourceTime">How long should this source be ignored</param>
    /// <returns>Was object damaged? No means the source is ignored or monster is dead</returns>
    public DamageType Damage(GameObject source, float damage = 1, bool ignoreInvulurability = false, float ignoreSourceTime = 0)
    {
        if (HP <= 0) return DamageType.None; // Already dead
        if (!source || !damageSources.ContainsKey(source) || Time.time - damageSources[source] > 0)
        {
            if (ignoreSourceTime > 0) damageSources[source] = Time.time + ignoreSourceTime;

            if (VulnerableCondition(source))
            {
                var wasHp = HP;
                if ((ReceiveFullDamage() || ignoreInvulurability))
                {
                    HP = Mathf.Max(minHpValue, HP - damage);
                    _FullHitEffect();
                }
                else
                {
                    HP = Mathf.Max(minHpValue, HP - (damage / 3f));
                    BulletAbsorb();
                }
                
                if (wasHp != HP)
                {
                    OnThisHit?.Invoke();
                    monsterDamaged.Invoke(wasHp - HP, gameObject);
                    aiAgent.SetDamaged();
                    HitEffect();
                }
                else UndamagedAnimation();
                
                if (HP <= 0) DestroyMonster(source, damage);
            }
            else
            {
                BulletAbsorb();
                OnThisAbsorb?.Invoke();
                return DamageType.Absorb;
            }
            return DamageType.Damaged;
        }
        return DamageType.None;
    }

    protected virtual void PreDestroyEffect()
    {
        usedNames.Remove(monsterName.text);
        if (enemyExplosionPrefab)
        {
            var enemyExplosion = PoolManager.GetPool(enemyExplosionPrefab, transform.position, Quaternion.identity);
            PoolManager.ReturnToPool(enemyExplosion, 3);
        }
    }

    public void FadeIn(float _fadeInTime)
    {
        fadeInTime = _fadeInTime;
        fadeInLeft = _fadeInTime;

        // Necessary to do before first frame
        UpdateFadeColor();
    }

    public float FadeInLeft
    {
        get => fadeInLeft;
    }

    private void UpdateFadeColor()
    {
        foreach (var sprite in sprites)
        {
            if (!sprite) continue;
            var newColor = sprite.color;
            newColor.a = Mathf.Lerp(1, 0, fadeInLeft / fadeInTime);
            sprite.color = newColor;
        }

        monsterName.color = Color.Lerp(Color.white, Color.clear, fadeInLeft / fadeInTime);
    }

    private void OnCollisionStay2D(Collision2D coll)
    {
        if (hitPlayerOnContact && fadeInLeft == 0 && coll.gameObject.tag == "Player")
        {
            if (!playerLife) playerLife = coll.gameObject.GetComponent<CharacterLife>();
            playerLife.Damage();
        }
    }

    public void MakeBoy()
    {
        THE_BOY = true;
        var enemyName = GetComponentInChildren<TMPro.TextMeshPro>();
        if (enemyName == null) return;
        enemyName.sortingLayerID = SortingLayer.NameToID("OnEffect");
    }

    public void MakeNoBoy()
    {
        THE_BOY = false;
        var enemyName = GetComponentInChildren<TMPro.TextMeshPro>();
        if (enemyName == null) return;
        enemyName.sortingLayerID = SortingLayer.NameToID("Default");
    }

    public bool isBoy()
    {
        return THE_BOY;
    }

    private void ChooseMyName()
    {
        if (!autoChooseName) return;
        List<string> possibleNames = evilDictionary.EvilNames();
        for (int i = 0; i < 200; i++) // Any ideas how to make this better?
        {
            var possibleName = possibleNames[Random.Range(0, possibleNames.Count)];
            if (!usedNames.Contains(possibleName))
            {
                usedNames.Add(possibleName);
                monsterName.text = possibleName;
                break;
            }
        }
    }

    public static void ClearUsedNames()
    {
        usedNames = new List<string>();
    }

    private GameObject BulletAbsorb()
    {
        invulnurabilityShield = PoolManager.GetPool(absorbPrefab, gameObject.transform.position, Quaternion.identity);
        invulnurabilityShield.transform.SetParent(gameObject.transform);
        PoolManager.ReturnToPool(invulnurabilityShield, 0.5f);
        return invulnurabilityShield;
    }
    
    /// <param name="percentage01Range">Should be between 0 and 1</param>
    public void SetMinHpPercentage(float percentage01Range)
    {
        if (percentage01Range > 1) Debug.LogError("Percentage parameter should be in range [0, 1]");
        SetMinHpValue(maxHP * percentage01Range);
    }

    public void SetMinHpValue(float minValue)
    {
        if (invulnurabilityShield)
        {
            PoolManager.ReturnToPool(invulnurabilityShield);
        }
        minHpValue = minValue;
    }

    private void UndamagedAnimation()
    {
        if (!invulnurabilityShield && absorbPrefab)
        {
            invulnurabilityShield = PoolManager.GetPool(absorbPrefab, gameObject.transform.position, Quaternion.identity);
            invulnurabilityShield.transform.SetParent(gameObject.transform);
            invulnurabilityShield.GetComponentInChildren<SpriteRenderer>().color = Color.black;
            PoolManager.ReturnToPool(invulnurabilityShield, 0.5f);
        }
    }

    private void HitEffect()
    {
        sprites[0].material.SetFloat("_TimeHit", Time.time);
    }

    private void DestroyMonster(GameObject source, float damage)
    {
        if (monsterManager != null)
            monsterManager.Death(gameObject);
        aiAgent.enabled = false;
        var behavs = GetComponentsInChildren<EnemyBehavior>();
        foreach (var behav in behavs)
        {
            behav.AgroBlock(999f);
        }
        GetComponentInChildren<UnityEngine.Collider2D>().enabled = false;
        hitPlayerOnContact = false;

        // Trigger an event for those who listen to it (if any)
        OnEnemyDead?.Invoke();
        EventManager.OnMonsterDead?.Invoke(transform.position);

        PreDestroyEffect();
        OnThisDead?.Invoke();
        
        StartCoroutine(DestoryGameObject(source, damage));
    }

    private IEnumerator DestoryGameObject(GameObject source, float damage)
    {
        if (invulnurabilityShield) invulnurabilityShield.transform.SetParent(null);
        Destroy(gameObject, 10f);

        var explodable = GetComponentInChildren<ExplosionForce>();
        if (explodable) explodable.DoExplosion(source ? source.transform.position : transform.position, Mathf.Clamp01(damage / maxHP));

        yield return new WaitForSeconds(timeKillToHideGObject);
        var renderers = GetComponentsInChildren<Renderer>();
        foreach (var rend in renderers)
        {
            rend.enabled = false;
        }
        yield return new WaitForSeconds(timeKillToDestroyGObject - timeKillToHideGObject);
        gameObject.SetActive(false);
    }

    private float minHpValue = 0;
    
    private float fadeInLeft;
    private SpriteRenderer[] sprites;
    private bool THE_BOY = false;
    private TMPro.TextMeshPro monsterName;
    private static List<string> usedNames = new List<string>();
    private GameObject invulnurabilityShield = null;

    private CharacterLife playerLife; // optimisation for collision stay

    public UnityEvent OnThisDead = new UnityEvent();
    public UnityEvent OnThisAbsorb = new UnityEvent();

    private Dictionary<GameObject, float> damageSources = new Dictionary<GameObject, float>();

    private AudioSource audioSource;
    private AIAgent aiAgent;
}
