using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponSkill : SkillBase
{
    public AudioClip attackSound;
    public AudioClip reloadSound;
    public float reloadTime = 3f;
    public float timeBetweenAttacks = 1f;
    public int ammoMagazine = 6;
    //public int bulletsLeft;

    public enum WeaponType { Automatic, Pistol, Heavy, Melee, Empty }
    public WeaponType weaponType;
    public static WeaponType[] weaponTypes = new WeaponType[] { WeaponType.Automatic, WeaponType.Pistol, WeaponType.Heavy, WeaponType.Melee, WeaponType.Empty };
    protected SkillManager.EquippedWeapon sourceGun;
    //public abstract void Shoot(Vector3 mousePos, Vector3 screenPoint);

    public override void InitializeSkill() { }

    public override void UpdateEffect() { }
    public virtual void UpdateEquippedEffect() { }

    public virtual void Attack(CharacterShooting attackManager, Vector3 mousePos, SkillManager.EquippedWeapon sourceGun) {
        this.sourceGun = sourceGun;
        Attack(attackManager,  mousePos);
    }
    public virtual void Attack(CharacterShooting attackManager, Vector3 mousePos) { }
    public virtual int AmmoConsumption() { return 1; }
    public virtual void EmptyClip() { }
}
