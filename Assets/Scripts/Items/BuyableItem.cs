using System;
using Game.Events;
using UnityEngine;

public class BuyableItem : PickupableItem
{
    public SkillBase itemAsset;

    private SkillManager skills;

    private void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        skills = player.GetComponent<SkillManager>();
        
        gameObject.tag = "BuyableItem";
    }

    protected override void PickUp(Collider2D player)
    {
        if (MoneyManager.MoneyAmount > 0)
        {
            EventManager.OnMoneyChange.Invoke(-1);
            EventManager.Notify($"Bought item \"{itemAsset.name}\"", 10);
            skills.AddSkill(itemAsset);
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
