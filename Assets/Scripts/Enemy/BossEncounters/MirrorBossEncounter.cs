using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorBossEncounter : BossEncounter
{
    [Header("General references")]
    public GameObject bossPrefab = null;
    public Transform bossSpawnPosition = null;
    public GameObject bossSpawnEffect = null;
    [Header("Phase 1 Attack 1 & 3")]
    public GameObject explosionProjectile = null;
    public Transform[] phase1MovePositions = null;
    [Header("Phase 1 Attack 2 & 4")]
    public GameObject miniBombProjectile = null;
    public ZoneScript miniBombTeleportPos = null;
    [Header("Phase 1 Attack 5")]
    public GameObject ellipseBulletPrefab = null;
    public Transform roomCenter = null;
    public Vector2 distanceFromCenterToUpRight = new Vector2(10, 10);

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

    [System.Serializable]
    private class ExplosionAttack : BossAttack
    {
        public ExplosionAttack(BossEncounter bossData, float attackLength, int projectilesCount, float waitBefore = 0, bool returnBack = true, bool allowInterruption = true, bool ended = false) 
            : base(bossData, attackLength, allowInterruption, ended)
        {
            this.projectilesCount = projectilesCount; 
            this.BD = bossData as MirrorBossEncounter;
            initialWait = waitBefore;
            wait = waitBefore;
        }

        protected override void AttackStart()
        {
            projectilePrefab = BD.explosionProjectile;
            normalBulletSpeed = projectilePrefab.GetComponent<EnemyBulletLife>().BulletSpeed;
            bossInstance = BD.bossInstance;

            bossSprite = bossInstance.GetComponentInChildren<SpriteRenderer>().gameObject;

            bulletMagnetTL = timeToBulletMagnet;

            base.AttackStart();
        }

        protected override void AttackUpdate()
        {
            base.AttackUpdate();
            if (wait <= 0)
            {
                wait = Mathf.Infinity;
                startPosition = bossInstance.transform.position;
                ExplodeTowardsPlayer();
            }
            if (endPosition != Vector3.zero)
            {
                //AdjustBulletSpeed();
                if (bulletMagnetTL > 0)
                {
                    bulletMagnetTL -= Time.deltaTime;
                    if (bulletMagnetTL <= 0)
                    {
                        MagnetAvoidBTPToBoss();
                    }
                }
                else if (magnetTime > 0)
                {
                    magnetTime -= Time.deltaTime;
                    if (magnetTime <= 0)
                    {
                        SplitBullets();
                    }
                }
                else
                {
                    //AdjustBulletSpeed();
                    bossInstance.transform.position = 
                        Vector3.Lerp(endPosition, startPosition, 
                            attackTimeLeft / (attackLength - initialWait - startingMagnetTime - timeToBulletMagnet));
                }
            }
            wait -= Time.deltaTime;
        }

        private void MagnetAvoidBTPToBoss()
        {
            magnetTime = startingMagnetTime;
            print("Magnetting back");
            foreach (var bullet in bullets)
            {
                bullet.BulletSpeed *= -1;
            }
        }

        private void SplitBullets()
        {
            print("Splitting bullets");
            var newBullets = new List<GameObject>();
            foreach (var bullet in bullets)
            {
                bullet.BulletSpeed *= -1f;
                var newBullet = Instantiate(bullet.gameObject, bullet.transform.position, Quaternion.Euler(0, 0, bullet.transform.eulerAngles.z + 60f));
                newBullets.Add(newBullet);
                bullet.transform.rotation = Quaternion.Euler(0, 0, bullet.transform.eulerAngles.z - 60);
            }
            foreach (var bullet in newBullets)
            {
                InitializeBullet(bullet);
            }
        }

        private void AdjustBulletSpeed()
        {
            var timeParamter = attackTimeLeft / (attackLength - initialWait);
            //var factor = normalBulletSpeed * Mathf.Max(minSpeed, (1 - Mathf.Pow(Mathf.InverseLerp(stopTimeRange.x, stopTimeRange.y, timeParamter), 3)));
            var factor = normalBulletSpeed * Mathf.Max(minSpeed, Mathf.Pow(timeParamter, 3));
            foreach (var bullet in bullets)
            {
                bullet.BulletSpeed = factor;
                print(factor);
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
                InitializeBullet(bullet);
            }

            ChooseEndMovePosition();
        }

        private void InitializeBullet(GameObject bullet)
        {
            BD.avoidBTP.Add(bullet.transform);
            var bulletLife = bullet.GetComponent<EnemyBulletLife>();
            bulletLife.BulletSpeed += Random.Range(-bulletSpeedVariation, bulletSpeedVariation);
            bullets.Add(bulletLife);
        }

        private void ChooseEndMovePosition()
        {
            // TODO: Сделать хитрее, чтобы не брались дистанции дальше X метров
            endPosition = BD.phase1MovePositions[Random.Range(0, BD.phase1MovePositions.Length)].position;
        }

        private void BossFadeAway()
        {

        }

        private void BossAppear()
        {

        }

        private float initialWait = 0;
        private float wait = 0;
        private int projectilesCount = 0;
        private GameObject projectilePrefab;
        private MirrorBossEncounter BD;
        private Transform bossInstance;
        private List<Transform> bulletsTransform = new List<Transform>();
        private List<EnemyBulletLife> bullets = new List<EnemyBulletLife>();
        private Vector3 startPosition;
        private Vector3 endPosition;
        
        private float bulletMagnetTL = 0f;
        private float timeToBulletMagnet = 0.2f;
        private float startingMagnetTime = 0.15f;
        private float magnetTime = 0;

        private const float angleVariation = 75f;
        private const float bulletSpeedVariation = 3f;
        private float normalBulletSpeed = 0;
        private float minSpeed = 0.2f;
        private Vector2 stopTimeRange = new Vector2(0.5f, 1.5f);

        private GameObject bossSprite;
        //private float appearTimeLeft = 0.3f;
        //private float fadeTimeLeft = 0.3f;
    }

    protected class MultibombAttack : BossAttack
    {
        public MultibombAttack(BossEncounter bossData, float attackLength, bool allowInterruption = true, bool ended = false) 
            : base(bossData, attackLength, allowInterruption, ended)
        {
            BD = bossData as MirrorBossEncounter;
            miniProjectilePrefab = BD.miniBombProjectile;
            teleportZone = BD.miniBombTeleportPos;
            teleportZone.UseZone();
        }

        protected override void AttackStart()
        {
            base.AttackStart();
            bossInstance = BD.bossInstance;
        }

        protected override void AttackUpdate()
        {
            bombPlaceTL -= Time.deltaTime;
            if (bombPlaceTL <= 0)
            {
                MinibombExplosion();
                bombPlaceTL = bombPlacePeriod;
            }

            UpdateIncreaseSpeed();

            base.AttackUpdate();
        }

        protected void MinibombExplosion()
        {
            for (int i = 0; i < bombProjectileCount; i++)
            {
                var angle = 360 / bombProjectileCount * i;
                var bullet = Instantiate(miniProjectilePrefab, bossInstance.position, Quaternion.Euler(0, 0, angle));
                miniBombBullets.Add(bullet.GetComponent<EnemyBulletLife>());
            }
            RandomTeleportInZone();
        }

        protected void RandomTeleportInZone()
        {
            bossInstance.transform.position = teleportZone.RandomZonePosition();
        }

        protected void UpdateIncreaseSpeed()
        {
            var multiplyBy = 1 + (increasePercentPerSecond * Time.deltaTime);
            foreach (var bullet in miniBombBullets)
            {
                bullet.BulletSpeed *= multiplyBy;
            }
        }

        protected GameObject miniProjectilePrefab;
        protected MirrorBossEncounter BD;
        protected Transform bossInstance;

        protected float bombPlacePeriod = 0.3f;
        protected float bombPlaceTL = 0;
        protected int bombProjectileCount = 8;
        protected float increasePercentPerSecond = 0.5f;
        protected ZoneScript teleportZone = null;
        protected List<EnemyBulletLife> miniBombBullets = new List<EnemyBulletLife>();
    }

    private class EllipseToCenterChaos : BossAttack
    {
        // Ellipse maths
        // We are using this formula for ellipse:
        //
        // x^2   y^2
        // --- + --- = (D/2)^2
        //  4     9
        //
        // This allows us to tweak D. The value of D means the right-most point of ellipse in Unity coordinates
        // So the starting D should be the right-most point of the arena
        // D lessens every frame so all projectiles move to the center of the room

        public EllipseToCenterChaos(BossEncounter bossData, float attackLength, bool allowInterruption = true, bool ended = false) : 
            base(bossData, attackLength, allowInterruption, ended)
        {
            BD = bossData as MirrorBossEncounter;
            ellipseProjectilePrefab = BD.ellipseBulletPrefab;
            upRightDistance = BD.distanceFromCenterToUpRight;
            roomCenter = BD.roomCenter.position;
        }

        protected override void AttackStart()
        {
            base.AttackStart();
            TestEllipse();
            Camera.main.GetComponent<CameraFocusOn>().FocusOn(roomCenter, attackLength, 2f);
        }

        protected override void AttackUpdate()
        {
            base.AttackUpdate();
        }

        protected override void AttackEnd()
        {
            base.AttackEnd();
            Camera.main.GetComponent<CameraFocusOn>().UnFocus(2);
        }

        private void TestEllipse()
        {
            var iCount = 17;
            for (float i = 0; i < iCount; i++)
            {
                SpawnBulletOnEllipseEdge(i / iCount * ellipseStartR, ellipseStartR, true);
            }
            for (float i = 0; i < 17; i++)
            {
                SpawnBulletOnEllipseEdge(i / iCount * ellipseStartR, ellipseStartR, false);
            }
            for (float i = 0; i < 17; i++)
            {
                SpawnBulletOnEllipseEdge(i / iCount * ellipseStartR * 0.5f, ellipseStartR / 2, true);
            }
        }

        private void SpawnBulletOnEllipseEdge(float xPos, float R, bool topSemisphere)
        {
            var yPos = Mathf.Sqrt(9 * (R * R - xPos * xPos) / 4);
            Instantiate(ellipseProjectilePrefab, roomCenter + new Vector3(topSemisphere ? yPos : -yPos, xPos, 0), Quaternion.identity);
        }

        private GameObject ellipseProjectilePrefab;
        private MirrorBossEncounter BD;
        private Transform bossInstance;

        private Vector3 roomCenter;
        private Vector2 upRightDistance;
        private GameObject ellipseInstance;
        private Vector2 ellipse = new Vector2(9, 4);
        private float ellipseStartR = 10;
        private Dictionary<Transform, float> bulletEllipseParameters = new Dictionary<Transform, float>();
        private Dictionary<Transform, bool> bulletEllipseSemisphere = new Dictionary<Transform, bool>();
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
            phaseLength = 50f;
            phaseType = PhaseType.TimeBased;
            attackOrder = AttackOrder.Sequence;
            attacks = new List<BossAttack>()
            {
                new ExplosionAttack(bossData, 2.4f, 9, waitBefore: 0.9f),
                new ExplosionAttack(bossData, 1.5f, 9),
                new ExplosionAttack(bossData, 1.5f, 9, returnBack: false),
                new MultibombAttack(bossData, 2.15f),  // 6 ticks
                new ExplosionAttack(bossData, 1.5f, 9),
                new ExplosionAttack(bossData, 1.5f, 9),
                new ExplosionAttack(bossData, 1.5f, 9, returnBack: false),
                new MultibombAttack(bossData, 4.1f),   // 10 ticks + end
                new EllipseToCenterChaos(bossData, 2.5f),
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
        //Camera.main.GetComponent<CameraFocusOn>().FocusOn(player.position, 3f, 2f);
    }

    public void StartFight()
    {
        encounterStarted = true;
        base.Start();
    }
}
