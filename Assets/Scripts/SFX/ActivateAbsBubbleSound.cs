using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateAbsBubbleSound : MonoBehaviour
{   
    void OnEnable()
    {
        var audio = GetComponent<AudioSource>();
        AudioManager.Play("Block", audio);
    } 
}
