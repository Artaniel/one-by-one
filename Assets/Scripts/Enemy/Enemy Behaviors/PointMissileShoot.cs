using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointMissileShoot : TimedShootWithOffset
{
    [Header("Point Missle Shot Parameters"), SerializeField] private float circleRadius = 3f;
    [SerializeField] private GameObject missilePointer = null;

    protected override GameObject ShootBullet(Vector2 direction, GameObject bulletToSpawn, float angleOffset)
    {
        var bullet = base.ShootBullet(direction, bulletToSpawn, angleOffset);
        var pointMissle = bullet.GetComponent<EnemyPointMissle>();
        var destination = (Vector2)(target.transform.position) + (Random.insideUnitCircle * circleRadius);
        pointMissle.SetDestination(destination);
        PoolManager.GetPool(missilePointer, destination, Quaternion.identity);
        return bullet;
    }
}
