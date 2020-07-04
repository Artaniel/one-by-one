using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Beetle switches between directed movement and
/// random X degrees (+/-) in local coordinates
/// </summary>
public class BeetleFace : LizardWaveFace
{
    public bool active = true;

    public float beetleRotationFixed = 0;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponentInChildren<Animator>();
        beetleRotationFixed = transform.eulerAngles.z + (Random.Range(60, 120) * Mathf.Sign(Random.value - 0.5f));
    }

    public void Switch()
    {
        active = !active;
        beetleRotationFixed = transform.eulerAngles.z + (Random.Range(60, 120) * Mathf.Sign(Random.value - 0.5f));
        if (active)
        {
            animator.Play("Beetle_stop");
            waveAmp = 40f;
        }
        else
        {
            animator.Play("Beetle_start");
            waveAmp = 0;
        }
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
}
