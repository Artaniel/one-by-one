using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter1BossMonsterLife : MonsterLife
{
    protected override bool SpecialConditions(GameObject source)
    {
        var mirrorComp = source.GetComponent<Chapter1BossInfusedBullet>();
        if (!mirrorComp) source.GetComponent<BulletLife>().piercing = true;

        return mirrorComp;
    }

    protected override void PreDestroyEffect()
    {
        base.PreDestroyEffect();
        AudioManager.Pause("Chapter1BossMusic",
            GameObject.FindWithTag("GameController").GetComponent<AudioSource>());
        var lightController = GameObject.FindGameObjectWithTag("GameController")
            .GetComponent<RoomLighting>();
        lightController.AddToLight(100);
    }

    protected override void HitEffect()
    {
        base.HitEffect();
    }
}
