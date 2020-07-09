using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedArcShot : TimedShootWithOffset
{
    [Header("Arc shot settings"), SerializeField]
    protected int shotCount = 2;
    [SerializeField]
    protected float angle = 15f;

    protected override void CompleteAttack()
    {
        Vector3 playerPos = target.transform.position;
        float inc = (2 * angle) / (shotCount - 1);
        for (float angleIt = -angle; angleIt <= angle; angleIt += inc)
        {
            float randomAngle = Random.Range(-randomShotAngle, randomShotAngle);
            ShootBullet(playerPos, bullet, angleIt + randomAngle);
        }

        if (shiftScript != null) shiftScript.DoShift();
    }
}
