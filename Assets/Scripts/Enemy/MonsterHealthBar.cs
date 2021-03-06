﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterHealthBar : HpBar
{
    protected override void Start()
    {
        base.Start();
        monsterLife = GetComponentInParent<MonsterLife>();
        monsterLife.OnThisHit.AddListener(HealthBarChange);
        monsterLife.OnThisDead.AddListener(Deactive);
    }

    protected override Vector2 GetCurrentMaxHp()
    {
        return new Vector2(monsterLife.HP, monsterLife.maxHP);
    }

    private MonsterLife monsterLife;
}
