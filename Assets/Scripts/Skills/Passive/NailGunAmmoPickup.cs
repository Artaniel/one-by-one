using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NailGunAmmoPickup : PickupableItem
{
    public SkillManager.EquippedWeapon nailGun;

    protected override void PickUp(Collider2D player)
    {
        if (nailGun != null) {
            if (nailGun.ammoLeft < nailGun.logic.ammoMagazine)
            {
                nailGun.ammoLeft++;
                PoolManager.ReturnToPool(gameObject);
            }
        }
    }

}
