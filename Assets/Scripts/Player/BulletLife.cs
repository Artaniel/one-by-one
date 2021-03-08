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
    protected float ignoreTime = 0.5f;
    public SkillManager.EquippedWeapon sourceGun = null;

    static BulletLife()
    {
        SceneManager.sceneLoaded += RefreshBulletsList;
    }

    protected virtual void Awake()
    {
        body = GetComponent<Rigidbody2D>();
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
    
    public virtual void InitializeBullet() {
        destroyed = false;
        copiedBullet = false;
        SetTimeLeft(timeToDestruction);
        coll2D.enabled = true;
        transform.localScale = startSize;
        BeginEmitter();
        ActivateSpawnMods();
        ApplyModsVFX();
        bullets.Add(gameObject);
    }

    public virtual void InitializeBullet(SkillManager.EquippedWeapon sourceGun)
    {
        this.sourceGun = sourceGun;
        InitializeBullet();
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

    protected virtual void OnTriggerEnter2D(Collider2D coll)
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
        body.velocity = transform.right * speed;
        ActivateMoveModsAfter();
    }

    protected virtual void EnemyCollider(Collider2D coll)
    {
        ActivateHitEnemyMods(coll);

        MonsterLife monsterComp = coll.GetComponentInParent<MonsterLife>();
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

    public virtual void DamageMonster(MonsterLife monster, float damageMultiplier = 1, BulletModifier initiator = null)
    {
        ActivateDamageEnemyMods(monster);

        bool damaged = monster.Damage(gameObject, damage * damageMultiplier * this.damageMultiplier, ignoreSourceTime: ignoreTime);
        if (monster.HP <= 0)
        {
            ActivateKillMods(monster);
        }
        if (initiator == null && damaged) // if the cause of "damage" is not a mod
        {
            if (monster.TryGetComponent(out AIAgent enemy))
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

    private List<BulletModifier> SortedMods()
    {
        if (listNotSorted)
        {
            bulletMods.Sort((x, y) => x.priority.CompareTo(y.priority));
            listNotSorted = false;
        }
        return bulletMods;
    }

    protected void ActivateHitEnemyMods(Collider2D coll) => SortedMods().ForEach(x => x.HitEnemyModifier(this, coll));

    protected void ActivateHitEnvironmentMods(Collider2D coll) => SortedMods().ForEach(x => x.HitEnvironmentModifier(this, coll));

    protected void ActivateDamageEnemyMods(MonsterLife enemy, BulletModifier initiator = null) => SortedMods().ForEach(x => x.DamageEnemyModifier(this, enemy));

    protected void ActivateSpawnMods() => SortedMods().ForEach(x => x.SpawnModifier(this));

    protected void ActivateDestroyMods() => SortedMods().ForEach(x => x.DestroyModifier(this));

    protected void ActivateKillMods(MonsterLife enemy) => SortedMods().ForEach(x => x.KillModifier(this, enemy));

    protected void MoveIfTiming(BulletModifier mod, BulletModifier.MoveTiming timing) {
        if (mod.moveTiming == timing) mod.MoveModifier(this);
    }

    protected void ActivateMoveModsBefore() => SortedMods().ForEach(x => MoveIfTiming(x, BulletModifier.MoveTiming.Preparation));

    protected void ActivateMoveModsAfter() => SortedMods().ForEach(x => MoveIfTiming(x, BulletModifier.MoveTiming.Final));

    protected void ApplyModsVFX()
    {
        foreach (var mod in SortedMods()) mod.ApplyVFX(this);      
    }

    protected void DeactivateMods()
    {
        foreach (var mod in SortedMods()) mod.DeactivateMod(this);
        bulletMods.Clear();
    }

    protected virtual void EnvironmentCollider(Collider2D coll)
    {
        ActivateHitEnvironmentMods(coll);

        if (!phasing)
        {
            DestroyBullet();
        }
    }

    public GameObject BulletFullCopy()
    {
        var bullet = PoolManager.GetPool(gameObject, transform.position, transform.rotation);
        var bulletComp = bullet.GetComponent<BulletLife>();
        bulletComp.speed = speed;
        bulletComp.damage = damage;
        
        bulletComp.bulletMods = new List<BulletModifier>();
        foreach (var mod in bulletMods)
        {
            bulletComp.AddMod(mod);
        }

        bulletComp.InitializeBullet();
        bulletComp.copiedBullet = true;

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
        body.velocity = Vector2.zero;
        destroyed = true;
        ActivateDestroyMods();
        coll2D.enabled = false;
        dynamicLightInOut?.FadeOut();
        StopEmitter();
        DeactivateMods();
        bullets.Remove(gameObject);
        PoolManager.ReturnToPool(gameObject, 1);
    }

    protected void BeginEmitter()
    {
        particlesEmitter?.Play(false);
        sprite.color = startColor;
        var emitterMain = particlesEmitter.main;
        emitterMain.startColor = emitterStartColor;
        bulletLight.color = lightStartColor;
    }

    protected void StopEmitter()
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
        emitterMain.startColor = newEmitterColor;
        Color lightColor = color + (bulletLight.color * 0.34f);
        bulletLight.color = lightColor;
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
    protected ParticleSystem particlesEmitter = null;
    protected Light2D bulletLight;
    public SpriteRenderer sprite = null;
    private Collider2D coll2D = null;
    protected DynamicLightInOut dynamicLightInOut = null;
    protected Color startColor;
    protected Color emitterStartColor;
    protected Color lightStartColor;
    protected bool destroyed = false;

    [System.NonSerialized]
    public Vector3 startSize = Vector3.one;

    private Rigidbody2D body;
}
