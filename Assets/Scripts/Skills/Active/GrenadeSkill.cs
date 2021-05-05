using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New grenade skill", menuName = "ScriptableObject/ActiveSkill/GrenadeSkill", order = 1)]
public class GrenadeSkill : ActiveSkill
{
    public GameObject projectile;

    protected override void ActivateSkill()
    {
        Vector3 target = CharacterShooting.GetCursor().position;
        Vector3 playerPosition = CharacterLife.player.transform.position;
        var grenade = PoolManager.GetPool(projectile, playerPosition, Quaternion.identity);
        Quaternion nextRotation = Quaternion.LookRotation(Vector3.forward, target - playerPosition);
        grenade.transform.rotation = nextRotation;
        grenade.transform.Rotate(0, 0, 90);
    }
}
