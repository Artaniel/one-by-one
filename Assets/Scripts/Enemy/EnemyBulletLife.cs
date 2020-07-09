using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyBulletLife : MonoBehaviour
{
    public float BulletSpeed = 12f;
    public float BulletLifeLength = 3f;
    public float ignoreCollisionTime = 0.35f;
    public bool phasing = false;
    public GameObject explosion;

    public UnityEvent bulletDestroyed = new UnityEvent();

    private void Awake()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
        dynamicLight = GetComponent<DynamicLightInOut>();
        lightFlicker = GetComponent<LightFlicker>();
        startingColor = sprite.color;
        hasExplosion = explosion;
    }

    protected virtual void OnEnable()
    {
        destroyed = false;
        sprite.color = startingColor;
        bulletLifeLeft = BulletLifeLength;
    }

    protected virtual void Update()
    {
        if (Pause.Paused || destroyed) return;

        Move();
        ignoreCollisionTime -= Time.deltaTime;
        bulletLifeLeft -= Time.deltaTime;
        if (bulletLifeLeft <= 0)
        {
            DestroyBullet();
        }
    }

    protected virtual void Move()
    {
        transform.Translate(Vector2.right * BulletSpeed * Time.deltaTime, Space.Self);
    }

    protected virtual void OnTriggerEnter2D(Collider2D coll)
    {
        if (ignoreCollisionTime > 0 || destroyed) return;
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

        if (hasExplosion) PoolManager.GetPool(explosion, transform.position, Quaternion.identity);
        
        bulletDestroyed.Invoke();
        dynamicLight?.FadeOut();
        lightFlicker?.Disable();
        PoolManager.ReturnToPool(gameObject, 0.5f);
    }

    private bool destroyed = false;
    private SpriteRenderer sprite;
    private Color startingColor;
    private float bulletLifeLeft;
    private DynamicLightInOut dynamicLight;
    private LightFlicker lightFlicker;
    
    private bool hasExplosion = false;
}
