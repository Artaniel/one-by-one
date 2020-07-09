using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointMissileShoot : TimedShootWithOffset
{
    [Header("Point Missle Shot Parameters"), SerializeField] private float circleRadius = 3f;

    protected override GameObject ShootBullet(Vector2 direction, GameObject bulletToSpawn, float angleOffset)
    {
        var bullet = base.ShootBullet(direction, bulletToSpawn, angleOffset);
        var pointMissle = bullet.GetComponent<EnemyPointMissle>();
        pointMissle.SetDestination((Vector2)(target.transform.position) + (Random.insideUnitCircle * circleRadius));
        return bullet;
    }
}
