using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicEnemyLaser : EnemyLaser
{
    [Header("Dynamic laser")]
    public Vector2 widthRange = new Vector2(0, 1);
    public float widthPhase = 1;
    private float timer = 0;
    
    private float sign = 1;

    void OnEnable()
    {
        timer = 0;
    }

    protected override void CustomUpdate()
    {
        timer += Time.deltaTime * sign;
        if (timer > widthPhase || timer < 0)
        {
            sign *= -1;
            timer += Time.deltaTime * sign; // to prevent back-and-forth
        }
        actualWidth = Mathf.Lerp(widthRange.x, widthRange.y, timer / widthPhase);
        line.widthMultiplier = actualWidth;
    }
}
