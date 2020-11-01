using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeTransparentIfPlayerEnters : MonoBehaviour
{
    private bool shouldBeTransparent;
    private SpriteRenderer[] spriteRenderers;
    private Color[] startColors;
    private Color[] destColors;
    private Color[] spriteColors;
    private float timeToTrans = 0.5f;
    private float timeToTransLeft = 0;
    private float timeToOpaq = 0.25f;
    private float timeToOpaqLeft = 0;

    void Start()
    {        
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        startColors = new Color[spriteRenderers.Length];
        destColors = new Color[spriteRenderers.Length];
        spriteColors = new Color[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
            startColors[i] = spriteRenderers[i].color;
        
    }

    void Update()
    {
        if (shouldBeTransparent && timeToTransLeft > 0)
        {
            timeToTransLeft = Mathf.Clamp01(timeToTransLeft - Time.deltaTime);
            for (int i = 0; i < spriteRenderers.Length; i++)
                spriteRenderers[i].color = Color.Lerp(destColors[i], spriteColors[i], timeToTransLeft / timeToTrans);
        }
        else if (!shouldBeTransparent && timeToOpaqLeft > 0)
        {
            timeToOpaqLeft = Mathf.Clamp01(timeToOpaqLeft - Time.deltaTime);
            for (int i = 0; i < spriteRenderers.Length; i++)
                spriteRenderers[i].color = Color.Lerp(startColors[i], spriteColors[i], timeToOpaqLeft / timeToOpaq);
        } 
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.CompareTag("Player"))
        {
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                if (spriteRenderers[i].color.a == 1)
                {
                    startColors[i] = spriteRenderers[i].color;
                    destColors[i] = new Color(startColors[i].r, startColors[i].g, startColors[i].b, 0.5f);
                }
                shouldBeTransparent = true;
                timeToTransLeft = timeToTrans;
                spriteColors[i] = spriteRenderers[i].color;
            }
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.CompareTag("Player"))
        {
            shouldBeTransparent = false;
            timeToOpaqLeft = timeToOpaq;
            for (int i = 0; i < spriteRenderers.Length; i++)
                spriteColors[i] = spriteRenderers[i].color;
        }
    }
}
