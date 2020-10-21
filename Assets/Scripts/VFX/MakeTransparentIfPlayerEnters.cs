using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeTransparentIfPlayerEnters : MonoBehaviour
{
    private bool shouldBeTransparent;
    private SpriteRenderer[] spriteRenderers;
    private Color startColor = Color.white;
    private Color destColor;
    private Color spriteColor;
    private float timeToTrans = 0.5f;
    private float timeToTransLeft = 0;
    private float timeToOpaq = 0.25f;
    private float timeToOpaqLeft = 0;

    void Start()
    {        
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        startColor = spriteRenderers[0].color;
    }

    void Update()
    {
        if (shouldBeTransparent && timeToTransLeft > 0)
        {
            timeToTransLeft = Mathf.Clamp01(timeToTransLeft - Time.deltaTime);
            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
                spriteRenderer.color = Color.Lerp(destColor, spriteColor, timeToTransLeft / timeToTrans);
        }
        else if (!shouldBeTransparent && timeToOpaqLeft > 0)
        {
            timeToOpaqLeft = Mathf.Clamp01(timeToOpaqLeft - Time.deltaTime);
            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
                spriteRenderer.color = Color.Lerp(startColor, spriteColor, timeToOpaqLeft / timeToOpaq);
        } 
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.CompareTag("Player"))
        {

            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                if (spriteRenderer.color.a == 1)
                {
                    startColor = spriteRenderer.color;
                    destColor = new Color(startColor.r, startColor.g, startColor.b, 0.5f);
                }
                shouldBeTransparent = true;
                timeToTransLeft = timeToTrans;
                spriteColor = spriteRenderer.color;
            }
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.CompareTag("Player"))
        {
            shouldBeTransparent = false;
            timeToOpaqLeft = timeToOpaq;
            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
                spriteColor = spriteRenderer.color;
        }
    }
}
