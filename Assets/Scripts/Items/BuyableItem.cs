using System;
using Game.Events;
using UnityEngine;

public class BuyableItem : PickupableItem
{
    public SkillBase itemAsset;

    private void Awake()
    {
        gameObject.tag = "BuyableItem";
    }

    protected override void PickUp(Collider2D player)
    {
        if (MoneyManager.MoneyAmount > 0)
        {
            EventManager.OnMoneyChange.Invoke(-1);
            EventManager.Notify($"Bought item \"{itemAsset.name}\"", 10);
        }
        else
        {
            EventManager.Notify("You can't buy this item", 1);
        }
    }

    private void OnMouseOver()
    {
        EventManager.Notify(itemAsset.description, 2);
    }
}
