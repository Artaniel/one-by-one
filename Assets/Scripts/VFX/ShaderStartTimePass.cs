using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderStartTimePass : MonoBehaviour
{
    void OnEnable()
    {
        GetComponent<SpriteRenderer>().material.SetFloat("_StartTime", Time.time);
    }
}
