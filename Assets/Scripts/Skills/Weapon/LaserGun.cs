using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LaserGun", menuName = "ScriptableObject/Weapon/LaserGun", order = 15)]
public class LaserGun : ShootingWeapon
{
    private bool laserIsShooting = false;
    protected CharacterShooting attackManager;
    private LaserRayBullet bullet;
         
    public override void UpdateEquippedEffect()
    {
        base.UpdateEquippedEffect();
        if (laserIsShooting)
        {
            if (!Input.GetButton("Fire1")) {
                StopRay();
            }
        }
    }

    public override void Attack(CharacterShooting attackManager, Vector3 mousePos)
    {
        this.attackManager = attackManager;
        if (!laserIsShooting) StartRay();
    }

    private void StartRay() {
        bullet = SpawnBulletTowardsCursor(attackManager.weaponTip, 0).GetComponent<LaserRayBullet>();
        bullet.startPoint = attackManager.weaponTip;
        laserIsShooting = true;
    }

    private void StopRay() {
        bullet?.DestroyBullet();
        laserIsShooting = false;
    }

    public override void EmptyClip() {
        StopRay();
    }
}
