using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleShooting : MonoBehaviour
{
    [SerializeField] GameObject bulletPrefab = null;
    private float timer = 0f;

    [SerializeField] private float openTime = 1f;
    [SerializeField] private float shootTime = 1f;
    [SerializeField] private float closeTime = 1f;
    [SerializeField] private float moveTime = 1f;
    [SerializeField] private int bulletsNumber = 20;
    private int bulletsWasShootCounter = 0;
    [SerializeField] private float ramdomAngleRange = 10f;

    [SerializeField] private Transform monsterSpriteObject = null;
    [SerializeField] private Animator spriteAnimation = null;
    [SerializeField] private Animator shadowAnimation = null;

    private enum Status { move, open, shoot, close }
    private Status status = Status.move;

    private GameObject player;
    private AIAgent agent;

    private MonsterLife monsterLife;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        agent = GetComponent<AIAgent>();
        status = Status.move;
        agent.moveSpeedMult = 1;
        agentSavedMaxRotation = agent.maxRotation;
        agentSavedVelocityFallback = agent.velocityFallBackPower;
        agentSavedKnockBackStability = agent.knockBackStability;
        if (SaveLoading.difficulty == 2)
        {
            bulletsNumber += 5;
        }
        audioSource = GetComponent<AudioSource>();
        hasShotAudio = audioSource.clip != null;
    }

    private void Update()
    {
        if (!Pause.Paused && monsterLife.HP > 0)
            if (status == Status.shoot)
            {
                timer -= Time.deltaTime;
                while ((shootTime - timer) / shootTime >= (float)bulletsWasShootCounter / (float)bulletsNumber)
                    ShootBullet(monsterSpriteObject);
                if (timer <= 0)
                {
                    //ainmation swich to close?
                    status = Status.close;
                    timer = closeTime;
                    bulletsWasShootCounter = 0;
                    spriteAnimation.Play("Attack-end");
                    shadowAnimation.Play("Attack-end");
                }
            }
            else if (status == Status.close)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    //ainmation swich to shoot?
                    status = Status.move;
                    timer = moveTime;
                    Unburrow();
                    spriteAnimation.Play("Pelmen-walking");
                    shadowAnimation.Play("Pelmen-walking");
                }
            }
            else if (status == Status.move)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    //ainmation swich to open?
                    status = Status.open;
                    timer = openTime;
                    agent.moveSpeedMult = 0;
                    spriteAnimation.Play("Attack-start");
                    shadowAnimation.Play("Attack-start");
                }
            }
            else if (status == Status.open)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    //ainmation swich to move?
                    status = Status.shoot;
                    timer = moveTime;
                    Burrow();
                    spriteAnimation.Play("Attack");
                    shadowAnimation.Play("Attack");
                }
            }
    }

    private void Burrow()
    {
        agent.maxRotation = 0;
        agent.velocityFallBackPower *= 3f;
        agent.knockBackStability *= 3f;
    }

    private void Unburrow()
    {
        agent.moveSpeedMult = 1;
        agent.maxRotation = agentSavedMaxRotation;
        agent.velocityFallBackPower = agentSavedVelocityFallback;
    }

    private void ShootBullet(Transform from = null) {
        Vector3 dirrectionToPlayer = player.transform.position - transform.position;
        float rotatingAngle = ((360f / bulletsNumber) * bulletsWasShootCounter) + Random.Range(-ramdomAngleRange, ramdomAngleRange) + 180;

        var spawnPos = from != null ? from.position : transform.position;
        GameObject bullet = PoolManager.GetPool(bulletPrefab, spawnPos, new Quaternion());

        if (hasShotAudio)
            AudioManager.Play("MonsterShot", audioSource);

        var angle = (Mathf.Atan2(dirrectionToPlayer.y, dirrectionToPlayer.x) * Mathf.Rad2Deg) + rotatingAngle;
        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);
        bulletsWasShootCounter++;        
    }

    private float agentSavedMaxRotation = 0;
    private float agentSavedVelocityFallback = 0;
    private float agentSavedKnockBackStability = 0;
    private bool hasShotAudio;
    private AudioSource audioSource;
}
