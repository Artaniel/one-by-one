﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ActiveMouseBulletSkill", menuName = "ScriptableObject/ActiveSkill/ActiveMouseBulletSkill", order = 1)]
public class ActiveMouseBullet : ActiveSkill
{
    public FollowCursorMod bulletMod;
    protected ActiveMouseBullet()
    {
        cooldownDuration = 5f;
        activeDuration = 3f;
    }

    protected override void ActivateSkill()
    {
        SkillManager.temporaryBulletMods.Add(bulletMod);
        ShootingWeapon.shootingEvents.AddListener(ReturnNormalBullets);
    }

    protected override void EndOfSkill()
    {
        ReturnNormalBullets();
    }

    private void ReturnNormalBullets()
    {
        if (SkillManager.temporaryBulletMods.Contains(bulletMod))
        {
            SkillManager.temporaryBulletMods.Remove(bulletMod);
        }

        ShootingWeapon.shootingEvents.RemoveListener(ReturnNormalBullets);
    }
}
