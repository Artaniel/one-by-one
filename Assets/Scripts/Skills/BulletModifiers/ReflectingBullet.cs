using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RicochetBulletMod", menuName = "ScriptableObject/BulletModifier/Ricochet", order = 1)]
public class ReflectingBullet : BulletModifier
{
    public override void SpawnModifier(BulletLife bullet)
    {
        base.SpawnModifier(bullet);
        bullet.phasing = true;
    }

    public override void HitEnvironmentModifier(BulletLife bullet, Collider2D coll)
    {
        base.HitEnvironmentModifier(bullet, coll);
        RaycastHit2D hit = Physics2D.Raycast(bullet.transform.position, bullet.transform.right, 5);
        if (hit)
        {
            Vector2 reflectDir = Vector2.Reflect(bullet.transform.right, hit.normal);
            float rot = Mathf.Atan2(reflectDir.y, reflectDir.x) * Mathf.Rad2Deg;
            bullet.transform.eulerAngles = new Vector3(0, 0, rot);
        }
    }
}
