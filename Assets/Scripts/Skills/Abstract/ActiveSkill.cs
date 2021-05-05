using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActiveSkill : SkillBase
{
    public float activeDuration = 3f;
    public float cooldownDuration = 5f;
    public AudioClip skillSound = null;
    public bool interrupt { get; private set; } = false;
    public bool allowActivationWhileActive = true;

    public override void _InitializeSkill()
    {
        interrupt = false;
        InitializeSkill();
    } 

    public virtual void InitializeSkill() { }

    public void _ActivateSkill()
    {
        interrupt = false;
        if (skillSound) AudioManager.Play(skillSound);
        ActivateSkill();
    }

    protected abstract void ActivateSkill();

    public override void UpdateEffect() { }

    public void _EndOfSkill() {
        EndOfSkill();
    }

    protected virtual void EndOfSkill() { }

    public void InterruptSkill() => interrupt = true;

    public override string GetDescription() =>
        $"{fullDescriprion}{(fullDescriprion == "" ? "" : "\n")}" +
        $"Cooldown: {cooldownDuration}\n" +
        $"{(activeDuration == 0 ? "" : $"Duration: {activeDuration}\n")}";
}
