using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerinoAggro : EnemyBehavior, ITwoModesSwitch
{
    EnemyBehavior[] behaviors;
    Animator animator;

    protected override void Start()
    {
        behaviors = GetComponents<EnemyBehavior>();
        animator = GetComponentInChildren<Animator>();
        GetComponent<ModeSwitcher>().AddSwitcheable(this);
        gameObject.tag = "Untagged";
    }

    void ITwoModesSwitch.Switch(bool mode)
    {
        animator.SetTrigger("Transform");
        StartCoroutine(DelayedSwitch());
    }

    IEnumerator DelayedSwitch()
    {
        yield return new WaitForSeconds(1f);
        GetComponentInChildren<TMPro.TextMeshPro>().enabled = true;

        gameObject.tag = "Enemy";
        foreach (var behav in behaviors)
        {
            behav.Activate();
        }
        
    }
}
