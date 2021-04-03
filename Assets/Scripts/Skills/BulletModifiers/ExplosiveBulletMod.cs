﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "ExplosiveBulletMod", menuName = "ScriptableObject/BulletModifier/ExplosiveBulletMod", order = 1)]
public class ExplosiveBulletMod : BulletModifier
{
    [SerializeField]
    protected float explosionRadius = 2f;

    [SerializeField]
    protected GameObject explosiveVfxPrefab;

    public override void HitEnemyModifier(BulletLife bullet, UnityEngine.Collider2D coll)
    {
        base.HitEnemyModifier(bullet, coll);
        ModEffect(bullet);
    }

    public override void HitEnvironmentModifier(BulletLife bullet, UnityEngine.Collider2D coll)
    {
        base.HitEnvironmentModifier(bullet, coll);
        ModEffect(bullet);
    }

    protected void ModEffect(BulletLife bullet)
    {
        UnityEngine.Collider2D[] monsters = FindMonsters(bullet);

        ExplosiveWave(monsters, bullet);
    }

    protected UnityEngine.Collider2D[] FindMonsters(BulletLife bullet)
    {
        UnityEngine.Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(bullet.transform.position, explosionRadius);
        var enemys = (from t in collider2Ds
                      where t.transform.tag == "EnemyCollider"
                      select t).ToArray();
        return enemys;
    }

    protected virtual void ExplosiveWave(UnityEngine.Collider2D[] enemys, BulletLife bullet)
    {
        var vfxPref = PoolManager.GetPool(explosiveVfxPrefab, bullet.transform.position, bullet.transform.rotation);
        foreach (var i in enemys)
        {
            var monsterLife = i.gameObject.GetComponentInParent<MonsterLife>();
            if (monsterLife)
            {
                var tmp = monsterLife.HP;
                bullet.DamageMonster(monsterLife, bullet.damage / 2);
            }
        }
    }

    protected virtual void Push(AIAgent enemy, float pushPower, Vector3 from)
    {
        Vector2 direction = enemy.transform.position - from;
        direction = direction.normalized * pushPower;
        enemy.KnockBack(direction);
    }
}
