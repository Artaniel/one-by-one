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

    public UnityEvent bulletDestroyed = new UnityEvent();

    protected virtual void Start()
    {
        Destroy(gameObject, BulletLifeLength);
    }

    protected virtual void Update()
    {
        if (Pause.Paused) return;

        transform.Translate(Vector2.right * BulletSpeed * Time.deltaTime, Space.Self);
        ignoreCollisionTime -= Time.deltaTime;
        BulletLifeLength -= Time.deltaTime;
        if (BulletLifeLength <= 0)
        {
            DestroyBullet();
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D coll)
    {
        if (ignoreCollisionTime > 0) return;
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
        bulletDestroyed.Invoke();
        Destroy(gameObject, 2 + BulletLifeLength);
        this.enabled = false;
    }
}
