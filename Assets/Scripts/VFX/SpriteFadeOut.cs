using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFadeOut : MonoBehaviour
{
    SpriteRenderer spriteRenderer = null;
    public float alphaDecreasePerSecond = 1f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    void Update()
    {
        var newc = spriteRenderer.color;
        newc.a -= alphaDecreasePerSecond * Time.deltaTime;
        spriteRenderer.color = newc;
    }
}
