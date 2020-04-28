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
            Instantiate(BD.bossSpawnEffect, BD.transform.position, Quaternion.identity);
        }

        protected override void AttackEnd()
        {
            BD.bossInstance = Instantiate(BD.bossPrefab, BD.transform.position, Quaternion.identity).transform;
        }

        private MirrorBossEncounter BD;
    }

    private class ExplosionAttack : BossAttack
    {
        public ExplosionAttack(BossEncounter bossData, float attackLength, int projectilesCount, bool returnBack = true, bool allowInterruption = true, bool ended = false) 
            : base(bossData, attackLength, allowInterruption, ended)
        {
            this.projectilesCount = projectilesCount; 
            this.BD = bossData as MirrorBossEncounter;
        }

        protected override void AttackStart()
        {
            projectilePrefab = BD.explosionProjectile;
            bossInstance = BD.bossInstance;

            base.AttackStart();

            Vector3 toPlayer = bossInstance.transform.position - BD.player.position;
            float angle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;
            Quaternion rotationEuler = Quaternion.AngleAxis(180 + angle, Vector3.forward);

            for (int i = 0; i < projectilesCount; i++)
            {
                BD.avoidBTP.Add(Instantiate(projectilePrefab, bossInstance.position, rotationEuler).transform);
            }
        }

        private int projectilesCount = 0;
        private GameObject projectilePrefab;
        private MirrorBossEncounter BD;
        private Transform bossInstance;
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
                new ExplosionAttack(bossData, 1.5f, 32),
                new ExplosionAttack(bossData, 1.5f, 32),
                new ExplosionAttack(bossData, 1.5f, 32, returnBack: false),
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
