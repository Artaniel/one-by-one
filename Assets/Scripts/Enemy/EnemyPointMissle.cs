using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPointMissle : EnemyBulletLife
{
    private float anglesPerSecond = 45f;

    protected override void OnEnable()
    {
        base.OnEnable();
        var offset = new Vector2(destination.x - transform.position.x, destination.y - transform.position.y);
        var angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
        var currentAngle = transform.rotation.eulerAngles.z;
        // В формуле учитываются: 
        // 1) градусы до цели
        // 2) (расстояние до цели) / 4 
        anglesPerSecond = Mathf.Abs(angle180fix(angle - currentAngle)) * (2f * BulletSpeed / Vector3.Distance(destination, transform.position));
    }

    protected override void Move()
    {
        if (Vector3.Distance(destination, transform.position) < 0.25f) DestroyBullet();
        
        var offset = new Vector2(destination.x - transform.position.x, destination.y - transform.position.y);
        var angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
        var currentAngle = transform.rotation.eulerAngles.z;
        var difference = angle180fix(angle - currentAngle);

        transform.rotation = Quaternion.Euler(0, 0, currentAngle + (Mathf.Sign(difference) * anglesPerSecond * Time.deltaTime));
        transform.Translate(Vector2.right * BulletSpeed * Time.deltaTime, Space.Self);
    }

    public void SetDestination(Vector3 destination)
    {
        this.destination = destination;
    }

    private float angle180fix(float angle)
    {
        if (angle > 180)
        {
            return -360 + angle;
        }
        else if (angle < -180)
        {
            return 360 + angle;
        }
        else return angle;
    }

    private Vector3 destination;
}
