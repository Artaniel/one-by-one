﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Events;

[ExecuteAlways]
public class PickupableSkill : PickupableItem
{
    public bool autoChangeIcon = true;
    public SkillBase skill;
    private Sprite sprite;

    protected override void Update()
    {
        base.Update();
        if (autoChangeIcon && sprite != skill.pickupSprite)
        {
            GetComponent<SpriteRenderer>().sprite = skill.pickupSprite;
            sprite = skill.pickupSprite;
        }
    }

    protected override void PickUp(UnityEngine.Collider2D player)
    {
        var skillInstance = Instantiate(skill);
        EventManager.Notify(LocalizationManager.GetTranlationENtoCurrent(skill.shortDescription), 1);
        player.GetComponent<SkillManager>().AddSkill(skillInstance);
        var canvas = GameObject.FindGameObjectWithTag("Canvas");
        if (canvas)
        {
            InventoryManager invM = canvas.GetComponentInChildren<InventoryManager>();
            if (invM)
            {
                Inventory inv = invM.inventory.GetComponent<Inventory>();
                if(inv.isStarted)
                    inv.AddSkill(skillInstance);
            }
        }
        Destroy(gameObject);
    }
}
