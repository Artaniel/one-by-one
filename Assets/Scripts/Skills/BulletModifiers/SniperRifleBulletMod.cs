using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SniperRifleBulletMod", menuName = "ScriptableObject/BulletModifier/SniperRifleBulletMod", order = 1)]
public class SniperRifleBulletMod : BulletModifier
{
    public override void SpawnModifier(BulletLife bullet)
    {
        var scale = bullet.transform.localScale;
        scale.y *= 0.5f;
        scale.x *= 1.2f;
        bullet.transform.localScale = scale;
        base.SpawnModifier(bullet);
    }

    public override void DeactivateMod(BulletLife bullet)
    {
        base.DeactivateMod(bullet);
        var scale = bullet.transform.localScale;
        scale.y *= (1 / 0.5f);
        scale.x *= (1 / 1.2f);
        bullet.transform.localScale = scale;
    }
}
