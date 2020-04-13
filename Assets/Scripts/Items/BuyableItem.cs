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
        if (MoneyManager.MoneyAmount < itemAsset.price) 
            EventManager.Notify("You don't have enough money", 1);
        else
        {
            EventManager.OnMoneyChange.Invoke(-itemAsset.price);
            EventManager.OnItemBought.Invoke(itemAsset);
            EventManager.Notify($"Bought item \"{itemAsset.name}\"", 10);
            skills.AddSkill(itemAsset);
            Destroy(gameObject);
        }
    }

    private void OnMouseOver()
    {
        EventManager.Notify(itemAsset.description, 2);
    }
}
