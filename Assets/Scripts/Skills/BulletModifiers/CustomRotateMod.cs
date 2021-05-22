using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New CustomRotateMod", menuName = "ScriptableObject/BulletModifier/Custom Rotation Timeline", order = 1)]
public class CustomRotateMod : BulletModifier
{
    public AnimationCurve animationCurve;

    public bool useCustomTime = false;
    public float customTimeMax = 1f;
    private float customTimePassed = 0f;

    private float bulletLength;

    public override void StartModifier(BulletLife bullet)
    {
        customTimePassed = 0;
        if (!useCustomTime)
        {
            bulletLength = bullet.TTDLeft > bullet.timeToDestruction ? bullet.TTDLeft : bullet.timeToDestruction;
        }
    }

    public override void ModifierUpdate(BulletLife bullet)
    {
        float timeParameter;
        if (useCustomTime)
        {
            timeParameter = customTimePassed / customTimeMax;
            customTimePassed += Time.deltaTime;
        }
        else
        {
            timeParameter = 1 - (bullet.TTDLeft / bulletLength);
        }
        float value = animationCurve.Evaluate(timeParameter);
        bullet.rigidbody.MoveRotation(bullet.rigidbody.rotation + value - previousValue);
        previousValue = value;
    }

    private float previousValue = 0;
}
