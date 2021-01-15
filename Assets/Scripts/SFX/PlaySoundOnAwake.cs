using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnAwake : MonoBehaviour
{
    [SerializeField]
    private bool shouldPlaySound = true;

    [SerializeField]
    private AudioClip audioClip = null;
    
    [Header("Non-default name?")]
    [Tooltip("Can be left blank if there is no need in specific name. Only works with audiosource")]
    [SerializeField]
    private string clipName = "";
    
    private AudioSource source;

    void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        if (shouldPlaySound)
        {
            if (audioClip)
            {
                AudioManager.Play(audioClip);
            }
            else
            {
                var name = clipName == "" ? source.clip.name : clipName;
                AudioManager.Play(name, source);
            }
        }    
    }
}
