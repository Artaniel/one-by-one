using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeetleModeSwitcher : Attack
{
    protected override void DoAttack()
    {
        if (Vector3.Distance(target.transform.position, transform.position) > 5f)
        {
            beetleFace.Switch();
            aiAgent.maxRotation *= beetleFace.active ? 0.4f : 2.5f;
            aiAgent.maxSpeed *= beetleFace.active ? 0.75f : (1f / 0.75f);
            cooldownLeft /= beetleFace.active ? 2f : 1f;
        }
        else
        {
            cooldownLeft /= 4f;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        beetleFace = GetComponent<BeetleFace>();
        aiAgent = GetComponent<AIAgent>();
        cooldownLeft /= 2f;
    }

    private BeetleFace beetleFace;
    private AIAgent aiAgent;
}
