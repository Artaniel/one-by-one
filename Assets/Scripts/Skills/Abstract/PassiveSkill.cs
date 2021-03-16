﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PassiveSkill : SkillBase
{
    public override void _InitializeSkill() {
        InitializeSkill();
    }
    public virtual void InitializeSkill() { }
    public override void UpdateEffect() { }
}
