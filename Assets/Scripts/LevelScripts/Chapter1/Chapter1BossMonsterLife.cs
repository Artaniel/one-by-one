﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter1BossMonsterLife : MonsterLife
{
    public bool hitNonMirror = true;

    protected override bool VulnerableCondition(GameObject source)
    {
        if (source)
        {
            var mirrorComp = source.GetComponent<Chapter1BossInfusedBullet>();

            if (!hitNonMirror && !mirrorComp) source.GetComponent<BulletLife>().piercing = true;

            return hitNonMirror || mirrorComp;
        }
        else
        {
            return false;
        }
    }

    protected override void PreDestroyEffect()
    {
        base.PreDestroyEffect();
        AudioManager.PauseSource("Chapter1BossMusic", null);
        var lightController = Labirint.GetCurrentRoom().GetComponent<RoomLighting>();
        lightController.AddToLight(100);
    }

    protected override void FullHitEffect()
    {
        base.FullHitEffect();
    }
}
