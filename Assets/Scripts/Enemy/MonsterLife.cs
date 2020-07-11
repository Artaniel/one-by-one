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
    public UnityEvent hpChangedEvent = new UnityEvent();

    [HideInInspector] public MonsterManager monsterManager = null;

    [SerializeField] private float timeKillToDestroyGObject = 0.15f;

    protected virtual bool Vulnerable()
    {
        return isBoy();
    }

    private void Awake()
    {
        HP = maxHP;
        
        sprites = GetComponentsInChildren<SpriteRenderer>();
        monsterName = GetComponentInChildren<TMPro.TextMeshPro>();

        ChooseMyName();
    }

    protected virtual void Start()
    {
        FadeIn(fadeInTime);
        sprites = GetComponentsInChildren<SpriteRenderer>();
        
        if (absorbPrefab == null)
        {
            absorbPrefab = Resources.Load<GameObject>("AbsorbBubble.prefab");
        }
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

    protected virtual bool SpecialConditions(GameObject source)
    {
        return true;
    }

    protected virtual void HitEffect() { }

    public void Damage(GameObject source, float damage = 1, bool ignoreInvulurability = false)
    {
        if (HP <= 0) return; // Already dead
        if (((THE_BOY && Vulnerable()) || ignoreInvulurability) && SpecialConditions(source))
        {
            var wasHp = HP;
            HP = Mathf.Max(minHpValue, HP - damage);
            if (wasHp != HP) hpChangedEvent?.Invoke();
            else UndamagedAnimation();

            if (HP <= 0) DestroyMonster(source, damage);
            else         HitEffect();
        }
        else
        {
            BulletAbsorb();
        }
    }

    protected virtual void PreDestroyEffect()
    {
        usedNames.Remove(monsterName.text);
        var enemyExplosion = Instantiate(enemyExplosionPrefab, transform.position, Quaternion.identity);
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
        if (absorbPrefab)
        {
            var absorb = Instantiate(absorbPrefab, gameObject.transform.position, Quaternion.identity);
            absorb.transform.SetParent(gameObject.transform);
            Destroy(absorb, 0.5f);
            return absorb;
        }
        return null;
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
            Destroy(invulnurabilityShield);
        }
        minHpValue = minValue;
    }

    private void UndamagedAnimation()
    {
        if (!invulnurabilityShield && absorbPrefab)
        {
            invulnurabilityShield = Instantiate(absorbPrefab, gameObject.transform.position, Quaternion.identity);
            invulnurabilityShield.transform.SetParent(gameObject.transform);
            invulnurabilityShield.GetComponentInChildren<SpriteRenderer>().color = Color.black;
            Destroy(invulnurabilityShield, 0.5f);
        }
    }

    private void DestroyMonster(GameObject source, float damage)
    {
        if (monsterManager != null)
            monsterManager.Death(gameObject);
        GetComponent<AIAgent>().enabled = false;
        GetComponentInChildren<Collider2D>().enabled = false;
        // Trigger an event for those who listen to it (if any)
        OnEnemyDead?.Invoke();
        EventManager.OnMonsterDead?.Invoke(transform.position);

        PreDestroyEffect();
        OnThisDead?.Invoke();
        var explodable = GetComponentInChildren<ExplosionForce>();
        if (explodable) explodable.DoExplosion(source ? source.transform.position : transform.position, Mathf.Clamp01(damage / maxHP));
        StartCoroutine(DestoryGameObject());
    }

    private IEnumerator DestoryGameObject()
    {
        yield return new WaitForSeconds(timeKillToDestroyGObject);
        Destroy(gameObject);
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
}
