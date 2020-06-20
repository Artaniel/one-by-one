using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IncreaseDamageMod", menuName = "ScriptableObject/BulletModifier/IncreaseDamageMod", order = 1)]
public class DamageModifier : BulletModifier
{
    [SerializeField] private float damageMultiplier = 0.1f;
    [SerializeField] private float sizeMultiplier = 1f;

    public override void SpawnModifier(BulletLife bullet)
    {
        base.SpawnModifier(bullet);

        bullet.AddToDamageMultiplier(damageMultiplier);

        if (!bullet.copiedBullet)
        {
            bullet.transform.localScale *= sizeMultiplier;
            var colliders = bullet.GetComponentsInChildren<BoxCollider2D>();
            foreach (var collider in colliders)
            {
                collider.size *= sizeMultiplier;
            }
        }
    }
}
