using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Beetle switches between directed movement and
/// random X degrees (+/-) in local coordinates
/// </summary>
public class BeetleFace : LizardWaveFace, ITwoModesSwitch
{
    public bool active = true;

    public float beetleRotationFixed = 0;

    protected override void Awake()
    {
        base.Awake();
        GetComponent<ModeSwitcher>().AddSwitcheable(this);
        aiAgent = GetComponent<AIAgent>();
        animator = GetComponentInChildren<Animator>();
        beetleRotationFixed = transform.eulerAngles.z + (Random.Range(60, 120) * Mathf.Sign(Random.value - 0.5f));
        if (active) Switch(active);
    }

    public void Switch(bool mode)
    {
        active = mode;
        beetleRotationFixed = transform.eulerAngles.z + (Random.Range(60, 120) * Mathf.Sign(Random.value - 0.5f));
        waveAmp = active ? 40 : 0;
        aiAgent.maxRotation *= active ? 0.4f : 2.5f;
        aiAgent.maxSpeed *= active ? 0.75f : (1f / 0.75f);
        if (active)
            animator.Play("Beetle_stop");
        else
            animator.Play("Beetle_start");
    }

    public override float GetRotation(float ampRotation = 0)
    {
        if (!isActive) return 0;
        
        if (active)
        {
            float desiredOrientation = -90 + beetleRotationFixed;
            float desiredRotation = desiredOrientation - agent.orientation;
            if (bypassAngleAccumulationSpeed != 0) desiredRotation += AccumulateBypassAngle();
            desiredRotation += WaveFluctuation();
            desiredRotation = MapToRange(desiredRotation);
            
            return desiredRotation / timeToTarget;
        }
        else
        {
            return base.GetRotation(ampRotation);
        }
    }

    private Animator animator;
    private AIAgent aiAgent;
}
