using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeetleLadybugMove : MonoBehaviour, ITwoModesSwitch
{
    [SerializeField] private bool active = false;
    [SerializeField] private float additionalSpeedMult = 1f;

    public void Awake()
    {
        aiAgent = GetComponent<AIAgent>();
        GetComponent<ModeSwitcher>().AddSwitcheable(this);
        animator = GetComponentInChildren<Animator>();
    }

    public void Switch(bool mode)
    {
        if (mode == active) return;
        active = mode;
        aiAgent.moveSpeedMult += additionalSpeedMult * (mode ? 1 : -1);
        if (active)
        {
            savedRotation = aiAgent.maxRotation;
            aiAgent.maxRotation = 0;
            animator.Play("Beetle_start");
        }
        else
        {
            aiAgent.maxRotation = savedRotation;
            animator.Play("Beetle_stop");
        }
    }
    
    private AIAgent aiAgent;
    private Animator animator;
    private float savedRotation = 0;
}
