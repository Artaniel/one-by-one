using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RicochetEnemyBulletLife : EnemyBulletLife
{
    public float decreaseSpeedByPercent = 0.3f;

    protected override void Update()
    {
        blockTimeAfterHit -= Time.deltaTime;
        base.Update();
    }

    protected override void OnTriggerEnter2D(UnityEngine.Collider2D coll)
    {
        if (ignoreCollisionTimeLeft > 0 || destroyed) return;

        if (block <= 0 && coll.gameObject.tag == "Environment")
        {
            Vector3 bulletPos = transform.position - transform.right * 0.1f;
            bulletPos.z = 0;
            Vector2 closestPoint = coll.ClosestPoint(bulletPos);
            var normal = (new Vector3(closestPoint.x, closestPoint.y) - bulletPos).normalized;

            Vector2 reflectDir = Vector2.Reflect(transform.right, normal);
            float rot = Mathf.Atan2(reflectDir.y, reflectDir.x) * Mathf.Rad2Deg;
            GetComponent<Rigidbody2D>().rotation = rot;
            BulletSpeed *= (1 - decreaseSpeedByPercent);
        }
        if (coll.gameObject.tag == "Player")
        {
            CharacterLife life = coll.gameObject.GetComponent<CharacterLife>();
            life.Damage();
        }
    }

    float block = 0;
    private float blockTimeAfterHit = 0.5f;
}
