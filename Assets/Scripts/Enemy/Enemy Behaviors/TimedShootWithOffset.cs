using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedShootWithOffset : TimedAttack
{
    public float randomShotAngle = 15f;
    public GameObject bullet = null;
    public Vector2 bulletSpawnOffset = new Vector2(0, 0);
    [SerializeField] protected bool isSpawnOffsetWorldCoordinates = true;
    [SerializeField] protected GameObject attackVFX = null;
    [SerializeField] protected bool rotationBased = false;

    protected override void Awake()
    {
        base.Awake();
        shiftScript = gameObject.GetComponent<ShiftAfterShoot>();
        audioSource = GetComponent<AudioSource>();
    }
    
    protected virtual GameObject ShootBullet(Vector2 direction, GameObject bulletToSpawn, float angleOffset)
    {
        var bullet = PoolManager.GetPool(
          bulletToSpawn, 
          transform.position, 
          rotationBased ? Quaternion.Euler(0, 0, transform.eulerAngles.z + 90 + angleOffset) : new Quaternion());
          
        bullet.GetComponent<EnemyBulletLife>().BulletSpeed *= attackSpeedModifier;

        audioSource.clip = attackSound;
        AudioManager.Play("MonsterShot", audioSource);

        if (!rotationBased)
        {
            var offset = new Vector2(direction.x - transform.position.x, direction.y - transform.position.y);
            var angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
            angle += angleOffset;
            bullet.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        
        bullet.transform.Translate(bulletSpawnOffset, isSpawnOffsetWorldCoordinates ? Space.World : Space.Self);
        return bullet;
    }

    protected override void AttackAnimation()
    {
        if (attackVFX != null)
        {
            attackAnimation = PoolManager.GetPool(
                attackVFX, 
                transform.position + new Vector3(bulletSpawnOffset.x, bulletSpawnOffset.y), 
                Quaternion.identity);
            attackAnimation.transform.SetParent(transform);
        }
    }

    protected override void CompleteAttack()
    {
        Vector3 playerPos = target.transform.position;

        float randomAngle = Random.Range(-randomShotAngle, randomShotAngle);
        ShootBullet(playerPos, bullet, randomAngle);

        if (shouldShift && shiftScript != null) shiftScript.DoShift();
    }

    private GameObject attackAnimation;

    protected ShiftAfterShoot shiftScript;
    [HideInInspector] public bool shouldShift = true;
    protected AudioSource audioSource;
}
