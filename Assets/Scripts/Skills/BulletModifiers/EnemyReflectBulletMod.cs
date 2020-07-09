using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TurnIntoEnemy Mod", menuName = "ScriptableObject/BulletModifier/TurnIntoEnemy", order = 1)]
public class EnemyReflectBulletMod : BulletModifier
{
    public override void ApplyModifier(BulletLife bullet)
    {
        base.ApplyModifier(bullet);

        bullet.piercing = true;
        // Make less collider size 
        var collider = bullet.GetComponent<BoxCollider2D>();
        collider.size *= 0.75f;

        enemyBullet = bullet.gameObject.AddComponent<EnemyBulletLife>();
        enemyBullet.phasing = true;
        enemyBullet.BulletSpeed = 0;
        enemyBullet.BulletLifeLength = Mathf.Infinity;
        enemyBullet.ignoreCollisionTime = 0;
    }

    public override void DeactivateMod(BulletLife bullet)
    {
        base.DeactivateMod(bullet);
        bullet.piercing = false;
        var collider = bullet.GetComponent<BoxCollider2D>();
        collider.size *= (1 / 0.75f);
        Destroy(enemyBullet);
    }

    private EnemyBulletLife enemyBullet;
}
