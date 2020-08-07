using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BowBulletMod", menuName = "ScriptableObject/BulletModifier/Bow BulletMod", order = 1)]
public class BowBulletMod : BulletModifier
{
    [HideInInspector]
    public float chargePower = 0.5f;

    public override void ApplyModifier(BulletLife bullet)
    {
        base.ApplyModifier(bullet);
        bullet.speed *= chargePower;
        bullet.AddToDamageMultiplier(chargePower - 1);
    }

    public override void DeactivateMod(BulletLife bullet)
    {
        base.DeactivateMod(bullet);
        bullet.speed *= (1 / chargePower);
        bullet.AddToDamageMultiplier(-chargePower + 1);
    }
}
