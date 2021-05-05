﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PiercingMod", menuName = "ScriptableObject/BulletModifier/PiercingMod", order = 1)]
public class PiercingBullet : BulletModifier
{
    public override void StartModifier(BulletLife bullet)
    {
        base.StartModifier(bullet);
        bullet.piercing = true;
    }

    public override void DeactivateMod(BulletLife bullet)
    {
        base.DeactivateMod(bullet);
        bullet.piercing = false;
    }
}
