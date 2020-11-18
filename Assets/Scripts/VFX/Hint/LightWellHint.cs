using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class LightWellHint : CurrentEnemyHint
{
    private SpriteRenderer spriteRenderer;
    private Color startColor;
    private Color transparentColor;

    private Light2D light2D;
    private float startIntensity;

    protected override void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        startColor = spriteRenderer.color;
        transparentColor = startColor;
        transparentColor.a = 0;

        light2D = GetComponentInChildren<Light2D>();
        startIntensity = light2D.intensity;
    }

    protected override void SetupHintVisual(Transform parent)
    {
        this.parent = parent;
    }

    protected override void UpdateVisual(float timeFraction)
    {
        if (parent)
        {
            transform.position = parent.position;
            spriteRenderer.color = Color.Lerp(transparentColor, startColor, timeFraction);
            light2D.intensity = Mathf.Lerp(0, startIntensity, timeFraction);
        }
        else
        {
            spriteRenderer.color = Color.Lerp(startColor, transparentColor, timeFraction);
            light2D.intensity = Mathf.Lerp(startIntensity, 0, timeFraction);
        }
    }
}
