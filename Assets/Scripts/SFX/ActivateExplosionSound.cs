using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateExplosionSound : MonoBehaviour
{  
    void OnEnable()
    {
        var audio = GetComponent<AudioSource>();
        AudioManager.Play("Explosion", audio);
    }
}
