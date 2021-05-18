using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyBulletLife : MonoBehaviour
{
    public float BulletSpeed = 12f;
    public Vector2 speedRandomRange = Vector2.zero;
    public float BulletLifeLength = 3f;
    public float ignoreCollisionTime = 0.35f;
    public bool phasing = false;
    public GameObject explosion;

    public class BulletDestroyedEvent : UnityEvent<EnemyBulletLife> { }

    public UnityEvent<EnemyBulletLife> bulletDestroyed = new BulletDestroyedEvent();

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        dynamicLight = GetComponent<DynamicLightInOut>();
        lightFlicker = GetComponent<LightFlicker>();
        startingColor = sprite.color;
        hasExplosion = explosion;
        startingBulletSpeed = BulletSpeed;
    }

    protected virtual void OnEnable()
    {
        destroyed = false;
        sprite.color = startingColor;
        bulletLifeLeft = BulletLifeLength;
        BulletSpeed = startingBulletSpeed + Random.Range(speedRandomRange.x, speedRandomRange.y);
        ignoreCollisionTimeLeft = ignoreCollisionTime;
        bulletDestroyed = new BulletDestroyedEvent();
    }

    protected virtual void Update()
    {
        if (Pause.Paused || destroyed) return;

        Move();
        ignoreCollisionTimeLeft -= Time.deltaTime;
        bulletLifeLeft -= Time.deltaTime;
        if (bulletLifeLeft <= 0)
        {
            DestroyBullet();
        }
    }

    protected virtual void Move()
    {
        body.velocity = transform.right * BulletSpeed;
    }

    protected virtual void OnTriggerEnter2D(UnityEngine.Collider2D coll)
    {
        if (ignoreCollisionTimeLeft > 0 || destroyed) return;
        if (coll.gameObject.tag == "Environment")
        {
            EnvironmentHit(coll);
        }
        else if (coll.gameObject.tag == "Player")
        {
            CharacterLife life = coll.gameObject.GetComponent<CharacterLife>();
            life.Damage();
        }
    }

    protected virtual void EnvironmentHit(Collider2D coll)
    {
        if (!phasing)
        {
            DestroyBullet();
        }
    }

    public virtual void DestroyBullet()
    {
        if (!destroyed)
        {
            destroyed = true;
            sprite.color = Color.clear;
            body.velocity = Vector2.zero;

            if (hasExplosion) PoolManager.GetPool(explosion, transform.position, transform.rotation);

            bulletDestroyed.Invoke(this);
            dynamicLight?.FadeOut();
            lightFlicker?.Disable();
            PoolManager.ReturnToPool(gameObject, 0.5f);
        }
    }

    public void UpdateLifeLeft(float newTimeLeft)
    {
        bulletLifeLeft = newTimeLeft;
    }

    protected bool destroyed = false;
    private SpriteRenderer sprite;
    private Color startingColor;
    protected float bulletLifeLeft;
    private DynamicLightInOut dynamicLight;
    private LightFlicker lightFlicker;
    
    private bool hasExplosion = false;
    private float startingBulletSpeed;
    protected Rigidbody2D body;
    protected float ignoreCollisionTimeLeft;
}
