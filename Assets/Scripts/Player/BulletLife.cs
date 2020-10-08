using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;
using UnityEngine.SceneManagement;

public class BulletLife : MonoBehaviour
{
    // Logic
    [Header("Default values: overrided by weapons")]
    public float speed = 8f;
    public float timeToDestruction = 1f;
    public float damage = 2f;
    public float TTDLeft = 0.5f;

    public List<BulletModifier> bulletMods = new List<BulletModifier>();

    public bool piercing = false;
    public bool phasing = false;
    public bool copiedBullet = false;
    public bool selfInit = false;

    public static List<GameObject> bullets = new List<GameObject>();

    static BulletLife()
    {
        SceneManager.sceneLoaded += RefreshBulletsList;
    }

    protected virtual void Awake()
    {
        audio = GetComponent<AudioSource>();
        bulletLight = GetComponentInChildren<Light2D>();
        coll2D = GetComponent<Collider2D>();
        dynamicLightInOut = GetComponent<DynamicLightInOut>();
        startColor = sprite.color;
        emitterStartColor = particlesEmitter.main.startColor.color;
        lightStartColor = bulletLight.color;
        startSize = transform.localScale;
    }

    protected void OnEnable()
    {
        if (selfInit) InitializeBullet();
    }

    public virtual void InitializeBullet()
    {
        destroyed = false;
        copiedBullet = false;
        AudioManager.Play("WeaponShot", audio);
        TTDLeft = timeToDestruction;
        coll2D.enabled = true;
        transform.localScale = startSize;
        BeginEmitter();
        ActivateSpawnMods();
        ApplyModsVFX();
        bullets.Add(gameObject);
    }

    void FixedUpdate()
    {
        if (Pause.Paused || destroyed) return;
        TTDLeft -= Time.fixedDeltaTime;
        Move();
        UpdateMods();
        
        if (TTDLeft < 0)
        {
            DestroyBullet();
        }
    }
    
    [System.NonSerialized]
    public float knockThrust = 10;
    [System.NonSerialized]
    public float knockTime = 0.5f;

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (destroyed) return;

        if (coll.CompareTag("EnemyCollider"))
        {
            EnemyCollider(coll);
        }
        else if (coll.tag == "Environment")
        {
            EnvironmentCollider(coll);
        }
    }

    protected virtual void Move()
    {
        ActivateMoveModsBefore();
        transform.Translate(Vector2.right * speed * Time.fixedDeltaTime);
        ActivateMoveModsAfter();
    }

    protected virtual void EnemyCollider(Collider2D coll)
    {
        ActivateHitEnemyMods(coll);

        // Damage
        var monsterComp = coll.gameObject.GetComponentInParent<MonsterLife>();
        if (monsterComp)
        {
            DamageMonster(monsterComp);
        }
        else
        {
            Debug.LogError("ОШИБКА: УСТАНОВИТЕ МОНСТРУ " + coll.gameObject.name + " КОМПОНЕНТ MonsterLife");
        }
        if (!piercing) DestroyBullet();
    }

    public void DamageMonster(MonsterLife monster, float damageMultiplier = 1, BulletModifier initiator = null)
    {
        ActivateDamageEnemyMods(monster);

        bool damaged = monster.Damage(gameObject, damage * damageMultiplier * this.damageMultiplier, ignoreSourceTime: 0.5f);
        if (monster.HP <= 0)
        {
            ActivateKillMods(monster);
        }
        if (initiator == null && damaged) // if the cause of "damage" is not a mod
        {
            // KnockBack
            var enemy = monster.GetComponent<AIAgent>();
            if (enemy != null)
            {
                KnockBack(enemy);
            }
        }
    }

    public void KnockBack(AIAgent enemy)
    {
        Vector2 direction = enemy.transform.position - transform.position;
        direction = direction.normalized * knockThrust * Time.fixedDeltaTime;
        enemy.KnockBack(direction);
    }

    // Bullet mods

    // Instantiates bullet mod and adds to mod list
    public BulletModifier AddMod(BulletModifier mod)
    {
        var modInstance = Instantiate(mod);
        bulletMods.Add(modInstance);
        listNotSorted = true;
        return modInstance;
    }

    private void UpdateMods()
    {
        for (int i = 0; i < bulletMods.Count; i++) 
        {
            bulletMods[i].ModifierUpdate(this);

            if (bulletMods[i].modifierTime <= 0)
            {
                bulletMods.RemoveAt(i);
                i--;
                listNotSorted = true;
            }
        }
    }

    private List<BulletModifier> SortedMods() {
        if (listNotSorted)
        {
            bulletMods.Sort((x, y) => x.priority.CompareTo(y.priority));
            listNotSorted = false;
        }
        return bulletMods;
    }

    private void ActivateHitEnemyMods(Collider2D coll)
    {
        foreach (var mod in SortedMods()) mod.HitEnemyModifier(this, coll);
    }

    private void ActivateHitEnvironmentMods(Collider2D coll)
    {
        foreach (var mod in SortedMods()) mod.HitEnvironmentModifier(this, coll);
    }

    private void ActivateDamageEnemyMods(MonsterLife enemy, BulletModifier initiator = null)
    {
        foreach (var mod in SortedMods())
        {
            if (mod != initiator) mod.DamageEnemyModifier(this, enemy);
        }
    }

    private void ActivateSpawnMods()
    {
        foreach (var mod in SortedMods()) mod.SpawnModifier(this);
    }

    private void ActivateDestroyMods()
    {
        foreach (var mod in SortedMods()) mod.DestroyModifier(this);
    }

    private void ActivateKillMods(MonsterLife enemy)
    {
        foreach (var mod in SortedMods()) mod.KillModifier(this, enemy);
    }

    private void ActivateMoveModsBefore()
    {
        foreach (var mod in SortedMods())
        {
            if (mod.moveTiming == BulletModifier.MoveTiming.Preparation) mod.MoveModifier(this);
        }
    }

    private void ActivateMoveModsAfter()
    {
        foreach (var mod in SortedMods())
        {
            if (mod.moveTiming == BulletModifier.MoveTiming.Final) mod.MoveModifier(this);
        }
    }

    private void ApplyModsVFX()
    {
        foreach (var mod in SortedMods()) mod.ApplyVFX(this);      
    }

    private void DeactivateMods()
    {
        foreach (var mod in SortedMods()) mod.DeactivateMod(this);
        bulletMods.Clear();
    }

    protected virtual void EnvironmentCollider(Collider2D coll)
    {
        ActivateHitEnvironmentMods(coll);

        if (coll.gameObject.GetComponent<Box>())
        {
            coll.gameObject.GetComponent<Box>().OnBullenHit();
        }

        if (coll.gameObject.GetComponent<MirrorWall>() != null)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right,
                float.PositiveInfinity, LayerMask.GetMask("Default"));
            if (hit)
            {
                Vector2 reflectDir = Vector2.Reflect(transform.right, hit.normal);
                float rot = Mathf.Atan2(reflectDir.y, reflectDir.x) * Mathf.Rad2Deg;
                transform.eulerAngles = new Vector3(0, 0, rot);
            }
        }
        else if (!phasing)
        {
            DestroyBullet();
        }
    }

    public GameObject BulletFullCopy()
    {
        var bullet = PoolManager.GetPool(gameObject, transform.position, transform.rotation);
        var bulletComp = bullet.GetComponent<BulletLife>();
        bulletComp.startColor = startColor;
        bulletComp.SetTimeLeft(timeToDestruction);
        bulletComp.speed = speed;
        bulletComp.damage = damage;
        bulletComp.copiedBullet = true;
        bulletComp.startSize = startSize;
        
        bulletComp.bulletMods = new List<BulletModifier>();
        foreach (var mod in bulletMods)
        {
            bulletComp.AddMod(mod);
        }

        bulletComp.InitializeBullet();

        return bullet;
    }

    public void SetTimeLeft(float timeLeft)
    {
        timeToDestruction = timeLeft;
        TTDLeft = timeLeft;
    }

    public virtual void DestroyBullet()
    {
        if (destroyed) return;
        destroyed = true;
        ActivateDestroyMods();
        coll2D.enabled = false;
        dynamicLightInOut?.FadeOut();
        StopEmitter();
        DeactivateMods();
        bullets.Remove(gameObject);
        PoolManager.ReturnToPool(gameObject, 1);
    }

    private void BeginEmitter()
    {
        particlesEmitter?.Play(false);
        sprite.color = startColor;
        var emitterMain = particlesEmitter.main;
        emitterMain.startColor = emitterStartColor;
        bulletLight.color = lightStartColor;
    }

    private void StopEmitter()
    {
        particlesEmitter?.Stop(false, ParticleSystemStopBehavior.StopEmitting);
        sprite.color = new Color(0, 0, 0, 0);
    }

    public void BlendSecondColor(Color color)
    {
        Color newColor = color + (sprite.color * 0.34f);
        sprite.color = newColor;
        var emitterMain = particlesEmitter.main;
        Color newEmitterColor = color + (emitterMain.startColor.color * 0.34f);
        emitterMain.startColor = newColor;
        Color lightColor = color + (bulletLight.color * 0.34f);
        bulletLight.color = newColor;
    }

    public void AddToDamageMultiplier(float addValue)
    {
        damageMultiplier += addValue;
    }

    private static void RefreshBulletsList(Scene scene, LoadSceneMode loadSceneMode)
    {
        bullets.Clear();
    }

    private bool listNotSorted = true;
    private float damageMultiplier = 1f;

    // Non-logic
    [SerializeField]
    private ParticleSystem particlesEmitter = null;
    private Light2D bulletLight;
    new private AudioSource audio;
    public SpriteRenderer sprite = null;
    private Collider2D coll2D = null;
    private DynamicLightInOut dynamicLightInOut = null;
    private Color startColor;
    private Color emitterStartColor;
    private Color lightStartColor;
    private bool destroyed = false;

    [System.NonSerialized]
    public Vector3 startSize = Vector3.one;
}
