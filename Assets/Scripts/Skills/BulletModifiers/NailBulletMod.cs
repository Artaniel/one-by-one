using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NailBulletMod", menuName = "ScriptableObject/BulletModifier/NailBulletMod", order = 12)]
public class NailBulletMod : BulletModifier
{
    private float maxSpeed;
    public float timeToFall = 2f;
    private NailGunAmmoPickup itemScript;

    public override void SpawnModifier(BulletLife bullet)
    {
        base.SpawnModifier(bullet);
        maxSpeed = bullet.speed;
        itemScript = bullet.GetComponent<NailGunAmmoPickup>();
        itemScript.nailGun = bullet.sourceGun;
        itemScript.enabled = false;
    }

    public override void ModifierUpdate(BulletLife bullet)
    {
        if (!itemScript.enabled && bullet.TTDLeft > (bullet.timeToDestruction - timeToFall))
            bullet.speed = (Mathf.Pow(1 - ((bullet.timeToDestruction - bullet.TTDLeft) / timeToFall), 8)) * maxSpeed;
        else
        {
            itemScript.enabled = true;
            bullet.speed = 0f;
        }
        base.ModifierUpdate(bullet);
    }

    public override void HitEnvironmentModifier(BulletLife bullet, UnityEngine.Collider2D coll)
    {
        bullet.speed = 0f;
        bullet.TTDLeft = bullet.timeToDestruction - timeToFall;
        bullet.phasing = true;
    }

}
