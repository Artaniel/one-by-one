using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardZone : MonoBehaviour
{
    public float ignoreTime = 0.5f;

    protected virtual void OnTriggerStay2D(Collider2D coll)
    {
        if (Time.time - ignoreTimeStamp < ignoreTime)
        {
            return;
        }

        if (coll.CompareTag("Player"))
        {
            ignoreTimeStamp = Time.time;
            HarmPlayer(coll.gameObject);
        }
    }

    protected virtual void HarmPlayer(GameObject player)
    {
        player.GetComponent<CharacterLife>().Damage();
    }

    protected float ignoreTimeStamp = 0;
}
