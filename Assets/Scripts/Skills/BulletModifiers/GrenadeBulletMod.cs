﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "GrenadeBulletMod", menuName = "ScriptableObject/BulletModifier/GrenadeBulletMod", order = 1)]
public class GrenadeBulletMod : ExplosiveBulletMod
{ 
    [Header("This Mod is OBSOLETE")]
    private float maxSpeed;

    public override void StartModifier(BulletLife bullet)
    {
        base.StartModifier(bullet);
        maxSpeed = bullet.speed;
    }

    public override void ModifierUpdate(BulletLife bullet)
    {
        bullet.speed = (1 - Mathf.Pow(1 - (bullet.TTDLeft / bullet.timeToDestruction), 3)) * maxSpeed;
        base.ModifierUpdate(bullet);
    }

    public override void DestroyModifier(BulletLife bullet)
    {
        base.DestroyModifier(bullet);
        ModEffect(bullet);
    }
}
