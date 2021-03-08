using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointMissileShoot : TimedShootWithOffset
{
    [Header("Point Missle Shot Parameters"), SerializeField] private float circleRadius = 3f;
    public bool focusOnPlayer = false;
    [SerializeField] private GameObject missilePointer = null;

    protected override GameObject ShootBullet(Vector2 direction, GameObject bulletToSpawn, float angleOffset)
    {
        var bullet = base.ShootBullet(direction, bulletToSpawn, angleOffset);
        var pointMissle = bullet.GetComponent<EnemyPointBullet>();
        var destination = 
            focusOnPlayer ? (Vector2)(target.transform.position) + (Random.insideUnitCircle * circleRadius)
                          : (Vector2)transform.position + (Vector2)(bullet.transform.right.normalized)
                                                        * Vector3.Distance(target.transform.position, transform.position) 
                                                        + (Random.insideUnitCircle * circleRadius);
        pointMissle.SetDestination(destination);
        PoolManager.GetPool(missilePointer, destination, Quaternion.identity);
        return bullet;
    }
}
