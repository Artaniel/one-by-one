using System;
using System.Collections;
using System.Collections.Generic;
using Game.Events;
using UnityEngine;

public class BuyableItem : PickupableItem
{
    public SkillBase itemAsset;

    private void OnTriggerExit2D(Collider2D other)
    {
        EventManager.OnNotify.Invoke("Exited", 2);
    }

    protected override void PickUp(Collider2D player)
    {
        if (MoneyManager.MoneyAmount > 0)
        {
            Debug.Log("Bought item");
            EventManager.OnMoneyChange.Invoke(-1);
        }
        else
        {
            // Debug.Log("You can't buy this item");
            EventManager.OnNotify.Invoke("You can't buy this item", 1);
        }
    }
}
