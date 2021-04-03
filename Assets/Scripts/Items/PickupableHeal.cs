using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupableHeal : PickupableItem
{
    public int healAmount = 1;
    protected CharacterLife characterLife;

    protected override void PickUp(UnityEngine.Collider2D player)
    {
        if (!characterLife) characterLife = player.GetComponent<CharacterLife>();
        if (characterLife.GetHp() < characterLife.GetMaxHp())
        {
            characterLife.Heal(healAmount);
            Destroy(gameObject);
        }
    }
}
