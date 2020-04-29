using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorBossEncounter : BossEncounter
{
    public GameObject bossPrefab = null;
    public Transform bossSpawnPosition = null;
    public GameObject bossSpawnEffect = null;
    public GameObject explosionProjectile = null;

    [HideInInspector] public Transform player;
    [HideInInspector] public Transform bossInstance;
    [HideInInspector] public List<Transform> avoidBTP = new List<Transform>(); // backtrack projectiles

    private class SpawnBossAttack : BossAttack
    {
        public SpawnBossAttack(BossEncounter bossData, float attackLength, bool allowInterruption = true, bool ended = false) : base(bossData, attackLength, allowInterruption, ended)
        {
            BD = bossData as MirrorBossEncounter;
        }

        protected override void AttackStart()
        {
            AudioManager.PauseMusic();
            Instantiate(BD.bossSpawnEffect, BD.bossSpawnPosition.position, Quaternion.identity);
        }

        protected override void AttackEnd()
        {
            AudioManager.PlayMusic(BD.GetComponent<AudioSource>());
            BD.bossInstance = Instantiate(BD.bossPrefab, BD.bossSpawnPosition.position, Quaternion.identity).transform;
        }

        private MirrorBossEncounter BD;
    }

    private class ExplosionAttack : BossAttack
    {
        public ExplosionAttack(BossEncounter bossData, float attackLength, int projectilesCount, float waitBefore = 0, bool returnBack = true, bool allowInterruption = true, bool ended = false) 
            : base(bossData, attackLength, allowInterruption, ended)
        {
            this.projectilesCount = projectilesCount; 
            this.BD = bossData as MirrorBossEncounter;
            wait = waitBefore;
        }

        protected override void AttackStart()
        {
            projectilePrefab = BD.explosionProjectile;
            normalBulletSpeed = projectilePrefab.GetComponent<EnemyBulletLife>().BulletSpeed;
            bossInstance = BD.bossInstance;

            base.AttackStart();
        }

        protected override void AttackUpdate()
        {
            base.AttackUpdate();
            if (wait <= 0)
            {
                wait = Mathf.Infinity;
                ExplodeTowardsPlayer();
            }
            wait -= Time.deltaTime;
        }

        private void MagnetAvoidBTPToBoss()
        {
            
        }

        private void AdjustBulletSpeed()
        {
            var timeParamter = attackLength - attackTimeLeft;
            var factor = normalBulletSpeed * Mathf.Max(minSpeed, (1 - Mathf.Pow(Mathf.InverseLerp(stopTimeRange.x, stopTimeRange.y, timeParamter), 3)));
            foreach (var bullet in bullets)
            {
                bullet.BulletSpeed = factor;
            }
        }

        private void ExplodeTowardsPlayer()
        {
            Vector3 toPlayer = bossInstance.transform.position - BD.player.position;
            float angle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;

            for (int i = 0; i < projectilesCount; i++)
            {
                var randomAngle = Random.Range(-angleVariation, angleVariation);
                var bullet = Instantiate(projectilePrefab, bossInstance.position, Quaternion.Euler(0, 0, angle + 180 + randomAngle));
                BD.avoidBTP.Add(bullet.transform);
                bullets.Add(bullet.GetComponent<EnemyBulletLife>());
            }
        }

        

        private float wait = 0;
        private int projectilesCount = 0;
        private GameObject projectilePrefab;
        private MirrorBossEncounter BD;
        private Transform bossInstance;
        private List<Transform> bulletsTransform = new List<Transform>();
        private List<EnemyBulletLife> bullets = new List<EnemyBulletLife>();

        private const float angleVariation = 75f;
        private float normalBulletSpeed = 0;
        private float minSpeed = 0.2f;
        private Vector2 stopTimeRange = new Vector2(0.5f, 1.5f);
    }

    public class InitialPhase : BossPhase
    {
        public InitialPhase(BossEncounter bossData) : base(bossData)
        {
            phaseName = "Initial phase";
            phaseLength = 3;
            phaseType = PhaseType.TimeBased;
            attackOrder = AttackOrder.Sequence;
            attacks = new List<BossAttack>()
            {
                new SpawnBossAttack(bossData, phaseLength)
            };
        }
    }

    public class AvoidancePhase : BossPhase
    {
        public AvoidancePhase(BossEncounter bossData) : base(bossData)
        {
            phaseName = "Avoidance";
            phaseLength = 10f;
            phaseType = PhaseType.TimeBased;
            attackOrder = AttackOrder.Sequence;
            attacks = new List<BossAttack>()
            {
                new ExplosionAttack(bossData, 2.5f, 18, waitBefore: 1),
                new ExplosionAttack(bossData, 1.5f, 18),
                new ExplosionAttack(bossData, 1.5f, 18, returnBack: false),
            };
        }
    }

    protected override void Start()
    {
        bossPhases = new List<BossPhase>()
        {
            new InitialPhase(this),
            new AvoidancePhase(this)
        };

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void StartFight()
    {
        encounterStarted = true;
        base.Start();
    }
}
