using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActiveSkill : SkillBase
{
    public float activeDuration = 3f;
    public float cooldownDuration = 5f;
    public AudioClip skillSound = null;

    public override void InitializeSkill() { }

    public virtual void _ActivateSkill()
    {
        if (skillSound) AudioManager.Play(skillSound);
        ActivateSkill();
    }

    public abstract void ActivateSkill();

    public override void UpdateEffect() { }

    public virtual void EndOfSkill() { }

    public override string GetDescription() =>
        $"{fullDescriprion}{(fullDescriprion == "" ? "" : "\n")}" +
        $"Cooldown: {cooldownDuration}\n" +
        $"{(activeDuration == 0 ? "" : $"Duration: {activeDuration}\n")}";
}
