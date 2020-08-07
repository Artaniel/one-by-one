using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AcidSpray))]
public class AcidSprayAfterDeath : MonoBehaviour
{
    void Start()
    {
        GetComponent<MonsterLife>().OnThisDead
            .AddListener(GetComponent<AcidSpray>().LaunchSpray);
    }
}
