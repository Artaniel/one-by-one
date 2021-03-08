using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPointBullet : EnemyBulletLife
{
    protected override void Move()
    {
        base.Move();
        if (Vector3.Distance(destination, transform.position) < 0.35f) DestroyBullet();
    }

    public void SetDestination(Vector3 destination)
    {
        this.destination = destination;
    }

    public Vector3 destination;
}
