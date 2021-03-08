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

    public UnityEvent bulletDestroyed = new UnityEvent();

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

    protected virtual void OnTriggerEnter2D(Collider2D coll)
    {
        if (ignoreCollisionTimeLeft > 0 || destroyed) return;
        if (coll.gameObject.tag == "Environment" && !phasing)
        {
            DestroyBullet();
        }
        else if (coll.gameObject.tag == "Player")
        {
            CharacterLife life = coll.gameObject.GetComponent<CharacterLife>();
            life.Damage();
        }
    }

    protected void DestroyBullet()
    {
        destroyed = true;
        sprite.color = Color.clear;

        if (hasExplosion) PoolManager.GetPool(explosion, transform.position, transform.rotation);
        
        bulletDestroyed.Invoke();
        dynamicLight?.FadeOut();
        lightFlicker?.Disable();
        PoolManager.ReturnToPool(gameObject, 0.5f);
    }

    public void UpdateLifeLeft(float newTimeLeft)
    {
        bulletLifeLeft = newTimeLeft;
    }

    protected bool destroyed = false;
    private SpriteRenderer sprite;
    private Color startingColor;
    private float bulletLifeLeft;
    private DynamicLightInOut dynamicLight;
    private LightFlicker lightFlicker;
    
    private bool hasExplosion = false;
    private float startingBulletSpeed;
    private Rigidbody2D body;
    protected float ignoreCollisionTimeLeft;
}
