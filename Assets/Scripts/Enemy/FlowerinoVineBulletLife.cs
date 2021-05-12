using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerinoVineBulletLife : EnemyBulletLife
{
    [HideInInspector]
    public PivotalMovement pivotalMovement;
    public float timeToSetup = 1.5f;

    bool goesBack = false;
    bool vineSetupped = false;
    float goesBackDuration;
    float goesBackTimeStamp;
    float initialBulletSpeed;
    Vector3 goesBackFrom = Vector3.zero;

    protected override void OnEnable()
    {
        base.OnEnable();
        goesBack = false;
        vineSetupped = false;
        initialBulletSpeed = BulletSpeed;
    }

    protected override void EnvironmentHit(Collider2D coll)
    {
        if (!vineSetupped)
        {
            SetupVine();
        }
    }

    protected void SetupVine()
    {
        if (!vineSetupped)
        {
            BulletSpeed = 0;
            pivotalMovement.pivots.Add(transform);
            vineSetupped = true;
        }
    }

    protected override void Update()
    {
        base.Update();
        if (!vineSetupped && BulletLifeLength > bulletLifeLeft + timeToSetup)
        {
            SetupVine();
        }
        if (goesBack)
        {
            body.position = Vector3.Lerp(goesBackFrom, pivotalMovement.transform.position, (Time.time - goesBackTimeStamp) / goesBackDuration);
        }
    }

    public override void DestroyBullet()
    {
        if (!goesBack)
        {
            goesBack = true;
            pivotalMovement.pivots.Remove(transform);
            goesBackDuration = (transform.position - pivotalMovement.transform.position).magnitude / initialBulletSpeed;
            goesBackTimeStamp = Time.time;
            goesBackFrom = transform.position;
            StartCoroutine(DelayedDestroy(goesBackDuration));
        }
    }

    private IEnumerator DelayedDestroy(float time)
    {
        yield return new WaitForSeconds(time);
        base.DestroyBullet();
    }
}
