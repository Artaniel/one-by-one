using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Shards On Hit Mod", menuName = "ScriptableObject/BulletModifier/ShardsOnHit", order = 1)]
public class ShardsOnHitBulletMod : BulletModifier
{
    [SerializeField] private int shardsCount = 3;

    [SerializeField, Header("Leave empty if bullet copied")]
    private GameObject shard;

    [SerializeField]
    private AudioClip shardsHitClip = null;

    private bool copiedOnce = false;

    public override void HitEnvironmentModifier(BulletLife bullet, UnityEngine.Collider2D coll)
    {
        base.HitEnvironmentModifier(bullet, coll);
        CreateShards(bullet, coll);
    }

    public override void HitEnemyModifier(BulletLife bullet, UnityEngine.Collider2D coll)
    {
        base.HitEnemyModifier(bullet, coll);
        CreateShards(bullet, coll);
    }

    private void CreateShards(BulletLife bullet, UnityEngine.Collider2D coll)
    {
        if (!bullet.copiedBullet && !copiedOnce)
        {
            copiedOnce = true;

            //Vector3 bulletPos = bullet.transform.position;
            Vector3 bulletPos = bullet.transform.position - bullet.transform.right;
            bulletPos.z = 0;
            Vector2 closestPoint = coll.ClosestPoint(bulletPos);
            var normal = (new Vector3(closestPoint.x, closestPoint.y) - bulletPos).normalized;

            Vector2 reflectDir = Vector2.Reflect(bullet.transform.right, normal);
            float initialRot = Mathf.Atan2(reflectDir.y, reflectDir.x) * Mathf.Rad2Deg;

            for (int i = 0; i < shardsCount; i++)
            {
                var newBullet = bullet.BulletFullCopy();

                float rot = initialRot + Random.Range(-45f, 45f);

                newBullet.transform.rotation = Quaternion.Euler(0, 0, rot);
                newBullet.GetComponent<Rigidbody2D>().position = newBullet.transform.position + (newBullet.transform.right * 0.5f);

                newBullet.transform.localScale *= 0.5f;
                var bulletLife = newBullet.GetComponent<BulletLife>();
                bulletLife.damage *= 0.5f;
                bulletLife.speed *= 1 + Random.Range(-0.25f, 0.25f);
                bulletLife.TTDLeft += Random.Range(-bulletLife.timeToDestruction * 0.6f, bulletLife.timeToDestruction * 0.2f);
            }

            if (shardsHitClip && bullet.audioSource)
                AudioManager.Play(shardsHitClip, bullet.audioSource);
        }
    }
}
