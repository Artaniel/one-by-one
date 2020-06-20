using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New First bullet shot passive", menuName = "ScriptableObject/PassiveSkill/FirstBulletShot", order = 1)]
public class FirstBulletShotPassive : PassiveSkill
{
    [SerializeField] public BulletModifier bulletModifier;

    private CharacterShooting characterShooting;

    public override void InitializeSkill()
    {
        base.InitializeSkill();
        var player = GameObject.FindGameObjectWithTag("Player");
        characterShooting = player.GetComponent<CharacterShooting>();

        characterShooting.firstBulletShot.AddListener(InfuseBullet);
    }

    private void InfuseBullet()
    {
        SkillManager.temporaryBulletMods.Add(bulletModifier);
        ShootingWeapon.shootingEvents.AddListener(ReturnNormalBullets);
    }

    private void ReturnNormalBullets()
    {
        if (SkillManager.temporaryBulletMods.Contains(bulletModifier))
        {
            SkillManager.temporaryBulletMods.Remove(bulletModifier);
        }

        ShootingWeapon.shootingEvents.RemoveListener(ReturnNormalBullets);
    }
}
