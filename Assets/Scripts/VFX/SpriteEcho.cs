using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteEcho : MonoBehaviour
{
    [Header("Place this script on a separate GameObject that contains all echoes and nothing else")]
    public float echoesAppearPeriod = 0.25f;
    public float startingAlpha = 0.65f;

    private SpriteRenderer[] spriteRenderers;
    private float echoesAppearTimeLeft = 0.25f;
    private int echoIndex = 0;

    void Start()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (var sr in spriteRenderers)
        {
            sr.transform.SetParent(null);
            sr.transform.position = transform.position;
        }
        echoesAppearTimeLeft = echoesAppearPeriod;
    }

    void Update()
    {
        echoesAppearTimeLeft -= Time.deltaTime;
        if (echoesAppearTimeLeft <= 0)
        {
            var newc = spriteRenderers[echoIndex].color;
            newc.a = startingAlpha;
            spriteRenderers[echoIndex].color = newc;
            spriteRenderers[echoIndex].transform.position = transform.position;
            spriteRenderers[echoIndex].transform.rotation = transform.rotation;
            echoIndex = (echoIndex + 1) % spriteRenderers.Length;

            echoesAppearTimeLeft = echoesAppearPeriod;
        }
    }
}
