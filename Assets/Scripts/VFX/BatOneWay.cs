using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatOneWay : MonoBehaviour
{
    public Vector2 speedRange = new Vector2(9f, 14f);
    public float speed;
    public Vector2 rotateSpeedRange = new Vector2(15f, 45f);
    private float rotateSpeed;
    public float rotateSign = 1f;
    public float timeToSignChange = 0.75f;
    private float timeToSignChangeLeft;

    public int startLayer = 3;
    public int activatedLayer = 6;
    public float timeToLayerSwitch = 0.5f;
    private float timeToLayerSwitchLeft;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        rotateSpeed = Random.Range(rotateSpeedRange.x, rotateSpeedRange.y);
        speed = Random.Range(speedRange.x, speedRange.y);

        timeToLayerSwitchLeft = timeToLayerSwitch;
        spriteRenderer.sortingOrder = startLayer;

        timeToSignChangeLeft = timeToSignChange;
    }
    
    void Update()
    {
        transform.Translate(transform.up * speed * Time.deltaTime, Space.World);
        transform.Rotate(0, 0, rotateSign * rotateSpeed * Time.deltaTime);
        timeToLayerSwitchLeft -= Time.deltaTime;
        if (timeToLayerSwitchLeft < 0)
        {
            timeToLayerSwitchLeft = 999f;
            spriteRenderer.sortingOrder = activatedLayer;
        }
        timeToSignChangeLeft -= Time.deltaTime;
        if (timeToSignChangeLeft < 0)
        {
            timeToSignChangeLeft = timeToSignChange;
            rotateSign *= -1;
        }
    }
}
