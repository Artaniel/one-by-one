using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusyBeetle : MonoBehaviour
{
    void OnEnable()
    {
        GetComponentInChildren<Animator>().Play("Beetle_start");
    }
}
