using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SniperRifleBulletMod", menuName = "ScriptableObject/BulletModifier/SniperRifleBulletMod", order = 1)]
public class SniperRifleBulletMod : BulletModifier
{
    public Vector2 scaleModifier = new Vector2(1.2f, 0.5f);

    public override void SpawnModifier(BulletLife bullet)
    {
        var scale = bullet.transform.localScale;
        scale.y *= scaleModifier.y;
        scale.x *= scaleModifier.x;
        bullet.transform.localScale = scale;
        base.SpawnModifier(bullet);
    }

    public override void DeactivateMod(BulletLife bullet)
    {
        base.DeactivateMod(bullet);
        var scale = bullet.transform.localScale;
        scale.y *= (1 / scaleModifier.y);
        scale.x *= (1 / scaleModifier.x);
        bullet.transform.localScale = scale;
    }
}
