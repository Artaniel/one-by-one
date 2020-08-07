using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Charging Weapon", menuName = "ScriptableObject/Weapon/Charging Weapon", order = 1)]
public class BowStyleWeapon : ShootingWeapon
{
    [Header("Charging weapon parameters")]
    public float minChargingTime = 0.5f;

    public override void InitializeSkill()
    {
        base.InitializeSkill();
        skillManager = Player.GetComponent<SkillManager>();
        characterMovement = Player.GetComponent<CharacterMovement>();
        charging = false;

        foreach (var bulletMod in bulletModifiers)
        {
            if (bulletMod is BowBulletMod) bowBulletMod = Instantiate(bulletMod as BowBulletMod);
        }
    }

    public override void Attack(CharacterShooting attackManager, Vector3 mousePos)
    {
        if (!charging) ChargeShot();
    }

    public override void UpdateEquippedEffect()
    {
        base.UpdateEquippedEffect();
        if (charging)
        {
            if (Input.GetButton("Fire1"))
            {
                chargingTime += Time.deltaTime;
            }
            else if (chargingTime > minChargingTime)
            {
                FinishAttack();
            }
            else
            {
                StopCharging();
            }
        }
    }

    protected void StopCharging()
    {
        reloadTime = savedReloadTime;
        charging = false;
        chargingTime = 0;

        characterMovement.AddToSpeedMultiplier(0.5f);
    }

    protected virtual void ChargeShot()
    {
        savedReloadTime = reloadTime;        
        reloadTime = 0.5f;
        charging = true;
        chargingTime = 0;

        characterMovement.AddToSpeedMultiplier(-0.5f);
    }

    protected virtual void FinishAttack()
    {
        var bullet = SpawnBulletTowardsCursor(CharacterShooting.GetCursor().position, Player.transform, 0).GetComponent<BulletLife>();
        if (bowBulletMod)
        {
            bowBulletMod.chargePower = Mathf.Clamp(chargingTime / minChargingTime, 1, 2);
            bowBulletMod.ApplyModifier(bullet);
            bullet.AddMod(bowBulletMod);
        }
        shootingEvents.Invoke();
        StopCharging();
    }

    protected SkillManager skillManager;
    protected CharacterMovement characterMovement;
    protected bool charging = false;
    protected float chargingTime = 0;
    protected float savedReloadTime;
    protected BowBulletMod bowBulletMod;
}
