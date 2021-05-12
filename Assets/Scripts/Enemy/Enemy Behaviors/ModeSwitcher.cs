using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeSwitcher : Attack
{
    public bool active = true;
    public float minDistanceToActivate = 5f;
    public float startCooldownReduce = 2f;

    public float trueSwitchCooldownReduce = 2f;
    public float falseSwitchCooldownReduce = 1f;

    protected override void DoAttack()
    {
        if (Vector3.Distance(target.transform.position, transform.position) > minDistanceToActivate)
        {
            Switch();
            cooldownLeft /= active ? trueSwitchCooldownReduce : falseSwitchCooldownReduce;
        }
        else
        {
            cooldownLeft /= 4f;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        cooldownLeft /= startCooldownReduce;
    }

    protected void Switch()
    {
        active = !active;
        switcheableBehaviours.ForEach(x => x.Switch(active));
    }

    public void AddSwitcheable(ITwoModesSwitch twoModesSwitch) => switcheableBehaviours.Add(twoModesSwitch);

    private List<ITwoModesSwitch> switcheableBehaviours = new List<ITwoModesSwitch>();
}
