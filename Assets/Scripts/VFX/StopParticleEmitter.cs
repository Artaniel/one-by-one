using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopParticleEmitter : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem particles = null;
    [SerializeField]
    public float TimeToDestroy = 0.5f;
    [SerializeField]
    private bool findAllInChildren = false;

    void Awake()
    {
        if (findAllInChildren)
        {
            allParticles = GetComponentsInChildren<ParticleSystem>();
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer) colorSaved = spriteRenderer.color;
    }

    void OnEnable()
    {
        stopped = false;
        timeToStopLeft = TimeToDestroy;
        if (spriteRenderer) spriteRenderer.color = colorSaved;
    }

    void StopSpecificParticle()
    {
        particles.Stop();
        if (spriteRenderer) spriteRenderer.color = new Color(0, 0, 0, 0);
    }

    void StopAllParticles()
    {
        foreach (var particle in allParticles)
        {
            particle.Stop();
        }
        
        if (spriteRenderer) spriteRenderer.color = new Color(0, 0, 0, 0);
    }

    void Update()
    {
        if (Pause.Paused) return;

        timeToStopLeft -= Time.deltaTime;
        if (timeToStopLeft < 0)
        {
            if (!stopped)
            {
                stopped = true;
                if (findAllInChildren)
                {
                    StopAllParticles();
                }
                else
                {
                    StopSpecificParticle();
                }
            }
        }
    }

    private bool stopped = false;
    private ParticleSystem[] allParticles = null;
    private float timeToStopLeft;
    private SpriteRenderer spriteRenderer;
    private Color colorSaved;
}
