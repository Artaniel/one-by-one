using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineAttack : TimedShootWithOffset
{
    public PivotalMovement pivotalMovement;
    public List<FlowerinoVineBulletLife> vines;

    protected override void Awake()
    {
        base.Awake();
        pivotalMovement = GetComponent<PivotalMovement>();
    }

    protected override void Start()
    {
        base.Start();
        GetComponent<MonsterLife>().OnThisDead.AddListener(DestroyAllBullets);
    }

    protected override void AttackAnimation()
    {
        return;
    }

    protected override GameObject ShootBullet(Vector2 direction, GameObject bulletToSpawn, float angleOffset)
    {
        var vine = base.ShootBullet(direction, bulletToSpawn, angleOffset);
        var vineLife = vine.GetComponent<FlowerinoVineBulletLife>();
        vines.Add(vineLife);
        vineLife.pivotalMovement = pivotalMovement;
        vineLife.bulletDestroyed.AddListener(RemoveFromList);

        var vineVisual = vine.GetComponentInChildren<ConnectedLine>();
        vineVisual.fromTransform = transform;

        return vine;
    }

    protected void RemoveFromList(EnemyBulletLife vine)
    {
        vines.Remove(vine as FlowerinoVineBulletLife);
    }

    protected void DestroyAllBullets()
    {
        foreach (var vine in vines)
        {
            vine.DestroyBullet();
        }
    }
}
