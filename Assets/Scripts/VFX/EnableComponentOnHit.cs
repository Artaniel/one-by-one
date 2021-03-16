using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableComponentOnHit : MonoBehaviour, IReactsToHit
{
    public MonoBehaviour[] monoBehaviours;

    void IReactsToHit.React()
    {
        foreach (var behav in monoBehaviours)
        {
            behav.enabled = false;
            behav.enabled = true;
        }
    }
}
