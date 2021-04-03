using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class Chapter1MirrorBulletInfuser : MonoBehaviour
{
    public bool infuseEnemyBullets = false;
    public Color enemyBulletColor = Color.magenta;

    private void OnTriggerEnter2D(UnityEngine.Collider2D coll)
    {
        if (coll.gameObject.GetComponent<BulletLife>() != null)
        {
            coll.gameObject.AddComponent<Chapter1BossInfusedBullet>();
            coll.gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.cyan;
            var emitter = coll.GetComponentInChildren<ParticleSystem>().main;
            emitter.startColor = Color.cyan;
            coll.gameObject.GetComponentInChildren<Light2D>().color = Color.cyan;
        }
        else
        {
            var eBulletLife = coll.gameObject.GetComponent<EnemyBulletLife>();
            if (infuseEnemyBullets && eBulletLife)
            {
                eBulletLife.ignoreCollisionTime = 0;
                eBulletLife.GetComponentInChildren<SpriteRenderer>().color = enemyBulletColor;
                eBulletLife.GetComponentInChildren<Light2D>().color = enemyBulletColor;
                // reflect
                RaycastHit2D hit = Physics2D.Raycast(eBulletLife.transform.position, transform.up,
                float.PositiveInfinity, LayerMask.GetMask("Default"));
                if (hit)
                {
                    Vector2 reflectDir = Vector2.Reflect(eBulletLife.transform.right, hit.normal);
                    float rot = Mathf.Atan2(reflectDir.y, reflectDir.x) * Mathf.Rad2Deg;
                    eBulletLife.transform.eulerAngles = new Vector3(0, 0, rot);
                }
            }
        }
    }
}
