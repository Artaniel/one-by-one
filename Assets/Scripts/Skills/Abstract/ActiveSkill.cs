using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActiveSkill : SkillBase
{
    public float activeDuration = 3f;
    public float cooldownDuration = 5f;

    public override void InitializeSkill() { }

    public abstract void ActivateSkill();

    public override void UpdateEffect() { }

    public virtual void EndOfSkill() { }
}
