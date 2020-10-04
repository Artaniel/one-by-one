using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class DynamicSpriteInOut : MonoBehaviour
{
    private SpriteRenderer sprite;
    private Color startColor;
    private Color transparentColor;
    [SerializeField] private float lifeSpanIn = 0.25f;
    [SerializeField] private float lifeSpanPause = 0f;
    [SerializeField] private float lifeSpanOut = 0.25f;
    private float lifeSpanLeftIn;
    private float lifeSpanPauseLeft;
    private float lifeSpanLeftOut;
    

    [SerializeField]
    private bool shouldFadeOut = true;

    // Start is called before the first frame update
    void Awake()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
        startColor = sprite.color;
        transparentColor = startColor;
        transparentColor.a = 0;
        shouldFadeOutParam = shouldFadeOut;
    }

    void OnEnable()
    {
        lifeSpanLeftIn = lifeSpanIn;
        lifeSpanPauseLeft = lifeSpanPause;
        lifeSpanLeftOut = lifeSpanOut;
        if (lifeSpanIn > 0)
        {
            sprite.color = transparentColor;
        }
        else
        {
            sprite.color = startColor;
        }
        shouldFadeOut = shouldFadeOutParam;
    }

    // Update is called once per frame
    void Update()
    {
        if (lifeSpanLeftIn > 0)
        {
            lifeSpanLeftIn -= Time.deltaTime;
            sprite.color = Color.Lerp(startColor, transparentColor, lifeSpanLeftIn / lifeSpanIn);
        }
        else if (lifeSpanPauseLeft > 0)
        {
            lifeSpanPauseLeft -= Time.deltaTime;
        }
        else if (shouldFadeOut)
        {
            lifeSpanLeftOut -= Time.deltaTime;
            sprite.color = Color.Lerp(transparentColor, startColor, lifeSpanLeftOut / lifeSpanOut);
        }
    }

    public void FadeOut()
    {
        shouldFadeOut = true;
    }

    private bool shouldFadeOutParam;
}
