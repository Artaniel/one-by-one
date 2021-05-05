using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GhostMode", menuName = "ScriptableObject/ActiveSkill/Ghost", order = 1)]
public class GhostMode : ActiveSkill
{
    protected GhostMode()
    {
        cooldownDuration = 5f;
        activeDuration = 3f;
    }

    protected override void ActivateSkill()
    {
        Debug.Log("Ghost");
    }
}
