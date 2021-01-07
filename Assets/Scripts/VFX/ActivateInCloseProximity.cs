using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateInCloseProximity : MonoBehaviour
{
    private Transform player;
    public GameObject[] toActivate;
    public float distanceToActivate = 5f;
    private bool activated = false;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;    
    }

    void Update()
    {
        if (!activated && Vector2.Distance(player.position, transform.position) < distanceToActivate)
        {
            activated = true;
            foreach (var obj in toActivate)
            {
                obj.SetActive(true);
            }
        }
    }
}
