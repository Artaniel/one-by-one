using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateAbsBubbleSound : MonoBehaviour
{
    new private AudioSource audio;

    void Awake()
    {
        audio = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        AudioManager.Play("Block", audio);
    } 
}
