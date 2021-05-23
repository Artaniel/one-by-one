using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardianBossEncounter : BossEncounter
{
    public GameObject attackWave;
    public AudioClip clawAttackAudio;

    public ZoneScript rockFallZone;
    public ZoneScript[] rainRockFallZones;
    public GameObject rockFallPredictor;
    public GameObject rockFalling;
    public int rockFallWave1 = 40;
    public int rockFallWave2 = 30;
    public AudioClip rockFallFliesAudio;

    public GameObject crystalThrowPredictor;
    public GameObject crystalToThrow;
    public Attack rockThrowAttack;

    public GameObject lavaCrystal;
    public GameObject lavaTrail;
    public GameObject sapphireCrystal;
    public GameObject sapphireShardsContainer;
    public GameObject emeraldCrystal;
    public GameObject emeraldSpikeContainer;
    public bool emeraldActive = false;
    public GameObject amethystCrystal;
    public GameObject amethystContainer;
    public GameObject onyxCrystal;
    public bool onyxActive = false;
    public Attack upgradedRockThrowAttack;
    public GameObject roomCenter;

    public Transform guardianMouth;
    public AudioClip crystalConsumeAudio;

    public Animator bodyAnimator;
    public Animator handAnimator;
    public Animator headAnimator;
    public Animator crystalAnimator;
    public GameObject slamEffect;
    public AudioClip rockFallSlamAudio;

    public AudioSource tramplingSource;

    [HideInInspector]
    public ShakeCameraExternal cameraShaker;
    [HideInInspector]
    public Transform player;

    public class Rockfall : BossAttack
    {
        public struct VolleyRock
        {
            public Transform transform;
            public Vector3 to;
            public float speed;
            public Collider2D hazardZone;
            public SpriteRenderer sprite;
            public float distance;

            public VolleyRock(Transform rock, Vector3 to, float speed, Collider2D hazardZone, SpriteRenderer sprite)
            {
                this.transform = rock;
                this.to = to;
                this.speed = speed;
                this.hazardZone = hazardZone;
                this.sprite = sprite;
                distance = Vector3.Distance(rock.position, to);
            }
        };

        public Rockfall(BossEncounter bossData, float attackLength, bool allowInterruption = true, bool ended = false)
            : base(bossData, attackLength, allowInterruption, ended)
        {
            this.bossData = bossData as GuardianBossEncounter;
            this.bossData.rockFallZone.UseZone();
            foreach (var zone in this.bossData.rainRockFallZones)
            {
                zone.UseZone();
            }
        }

        protected override void AttackStart()
        {
            bossData.handAnimator.SetTrigger("Slam");
            AudioManager.Play(bossData.rockFallSlamAudio);
            volleyOne.Clear();
            volleyTwo.Clear();

            bossData.StartCoroutine(DelayedRockFall(0.65f, bossData.rockFallWave1, volleyOne));
            bossData.StartCoroutine(DelayedSlamEffect(0.5f));
            if (bossData.onyxActive) bossData.StartCoroutine(DelayedRockFall(1.15f, bossData.rockFallWave2, volleyTwo));
        }

        private void RockFall(int count, List<VolleyRock> volleyZone)
        {
            float farthestDist1 = 0, farthestDist2 = 0;
            ZoneScript farthestZone1 = null, farthestZone2 = null;
            foreach (var zoneIt in bossData.rainRockFallZones)
            {
                float zoneToPlayer = Vector3.Distance(zoneIt.transform.position, bossData.player.position);
                if (zoneToPlayer > farthestDist1)
                {
                    farthestDist2 = farthestDist1;
                    farthestZone2 = farthestZone1;
                    farthestDist1 = zoneToPlayer;
                    farthestZone1 = zoneIt;
                }
                else if (zoneToPlayer > farthestDist2)
                {
                    farthestDist2 = zoneToPlayer;
                    farthestZone2 = zoneIt;
                }
            }
            ZoneScript zone = Random.Range(0, 2) == 0 ? farthestZone1 : farthestZone2;

            for (int i = 0; i < count; i++)
            {
                Vector3 rockFrom = zone.RandomZonePosition3();
                var rock = PoolManager.GetPool(bossData.rockFalling, rockFrom, Quaternion.identity);
                Vector3 rockDirection = bossData.rockFallZone.RandomZonePosition3();
                PoolManager.GetPool(bossData.rockFallPredictor, rockDirection, Quaternion.identity);
                VolleyRock volleyRock = new VolleyRock(rock.transform, rockDirection, 48f, rock.GetComponent<Collider2D>(), rock.GetComponentInChildren<SpriteRenderer>());
                volleyRock.hazardZone.enabled = false;
                volleyRock.sprite.sortingOrder = 10;

                volleyZone.Add(volleyRock);
                PoolManager.ReturnToPool(rock, 1.6f);
            }
            bossData.cameraShaker.ShakeCamera();
            AudioManager.Play(bossData.rockFallFliesAudio);
        }

        // Called in phase update to continue rockfall even if attack ended
        public void UpdateVolley(List<VolleyRock> volley)
        {
            for (int i = 0; i < volley.Count; i++)
            {
                var rock = volley[i];
                if (Vector3.Distance(rock.transform.position, rock.to) > 0.001f)
                {
                    rock.transform.position = Vector3.MoveTowards(rock.transform.position, rock.to, rock.speed * Time.deltaTime);
                    rock.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 10, Vector3.Distance(rock.transform.position, rock.to) / rock.distance);
                }
                else
                {
                    rock.hazardZone.enabled = true;
                    rock.sprite.sortingOrder = 0;
                }
            }
        }

        private IEnumerator DelayedRockFall(float delay, int count, List<VolleyRock> volleyZone)
        {
            yield return new WaitForSeconds(delay);
            RockFall(count, volleyZone);
        }

        private IEnumerator DelayedSlamEffect(float delay)
        {
            yield return new WaitForSeconds(delay);
            Vector3 slamOffset = bossData.transform.up * 5f + bossData.transform.right * 3f;
            var slamEffect = PoolManager.GetPool(bossData.slamEffect, bossData.transform.position + slamOffset, bossData.transform.rotation);
            PoolManager.ReturnToPool(slamEffect, 0.3f);
        }

        GuardianBossEncounter bossData;
        public List<VolleyRock> volleyOne = new List<VolleyRock>();
        public List<VolleyRock> volleyTwo = new List<VolleyRock>();
    }

    public class FistAttack : BossAttack
    {
        public FistAttack(BossEncounter bossData, float attackLength, bool allowInterruption = true, bool ended = false) 
            : base(bossData, attackLength, allowInterruption, ended) {
            this.bossData = bossData as GuardianBossEncounter;
            aiAgent = this.bossData.GetComponent<AIAgent>();
            attackWave = this.bossData.attackWave;
            attackWave.transform.SetParent(null);
            particles = attackWave.GetComponentsInChildren<ParticleSystem>();
            waitBeforeAttack = 0.5f;
            waitAfterAttack = this.bossData.onyxActive ? 0.5f : 0.75f;
        }

        protected override void AttackStart()
        {
            realAttackDuration = attackLength - waitAfterAttack - waitBeforeAttack;
            rotateSpeed = (bossData.onyxActive ? 360f : 120f) / realAttackDuration;

            delayedStartCoroutine = bossData.StartCoroutine(DelayedStart());
            savedSpeed = aiAgent.moveSpeedMult;
            aiAgent.moveSpeedMult *= 2f;
            savedRotation = aiAgent.maxRotation;
            aiAgent.maxRotation = 30f;

            bossData.PlayRunAnimation();
        }

        private IEnumerator DelayedStart()
        {
            yield return new WaitForSeconds(waitBeforeAttack - 0.1f); // Because otherwise update happens

            if (ended) yield return null;

            bossData.handAnimator.SetTrigger("Grab");
            bossData.GetComponent<AIAgent>().moveSpeedMult /= 100 * 2f;
            attackWave.transform.SetPositionAndRotation(bossData.transform.position, bossData.transform.rotation);
            attackWave.transform.position = bossData.transform.position + bossData.transform.up * 2f;
            attackWave.transform.Rotate(0, 0, -rotateSpeed * 0.5f * realAttackDuration);
            attackWave.transform.GetChild(0).gameObject.SetActive(true);

            attackWave.GetComponentInChildren<TrailRenderer>().Clear();
            ToggleParticles(true);

            bossData.PlayIdleAnimation();
            AudioManager.Play(bossData.clawAttackAudio);
        }

        protected override void AttackUpdate()
        {
            if (attackLength - attackTimeLeft < waitBeforeAttack) return;

            if (attackTimeLeft > waitAfterAttack)
            {
                attackWave.transform.Rotate(0, 0, rotateSpeed * Time.deltaTime, Space.Self);
            }

            if (attackTimeLeft < waitAfterAttack && particlesOn)
            {
                ToggleParticles(false);
            }
        }

        protected override void AttackEnd()
        {
            bossData.StopCoroutine(delayedStartCoroutine);

            ToggleParticles(false); // Double check (for force stop)
            attackWave.transform.GetChild(0).gameObject.SetActive(false);
            aiAgent.moveSpeedMult = savedSpeed;
            aiAgent.maxRotation = savedRotation;

            bossData.PlayRunAnimation();
        }

        private void ToggleParticles(bool on)
        {
            particlesOn = on;
            foreach (var particle in particles)
            {
                if (on) particle.Play();
                else particle.Stop();
            }
        }

        bool particlesOn = false;
        GuardianBossEncounter bossData;
        float rotateSpeed;
        float savedRotation;
        float waitAfterAttack = 0.25f;
        float waitBeforeAttack = 0.5f;
        float realAttackDuration;
        GameObject attackWave;
        AIAgent aiAgent;
        Coroutine delayedStartCoroutine;
        private float savedSpeed;
        ParticleSystem[] particles;
    }

    public class ThrowAttack : BossAttack
    {
        public ThrowAttack(BossEncounter bossData, float attackLength, bool allowInterruption = true, bool ended = false) 
            : base(bossData, attackLength, allowInterruption, ended)
        {
            this.bossData = bossData as GuardianBossEncounter;
            aiAgent = this.bossData.GetComponent<AIAgent>();
        }

        protected override void AttackStart()
        {
            aiAgent.maxSpeed /= 100;
            savedRotation = aiAgent.maxRotation;
            aiAgent.maxRotation = 0;
            if (bossData.onyxActive)
                bossData.upgradedRockThrowAttack.ForceAttack();
            else
                bossData.rockThrowAttack.ForceAttack();
            bossData.crystalAnimator.SetTrigger("Attack");
            bossData.handAnimator.SetTrigger("Throw");
            bossData.PlayIdleAnimation();
        }

        protected override void AttackUpdate()
        {
            if (attackTimeLeft / attackLength < 0.5f)
            {
                aiAgent.maxRotation = savedRotation;
            }
        }

        protected override void AttackEnd()
        {
            aiAgent.maxSpeed *= 100;
            aiAgent.maxRotation = savedRotation;
            bossData.PlayRunAnimation();
        }

        float savedRotation;
        AIAgent aiAgent;
        GuardianBossEncounter bossData;
    }

    public class GuardianBattle : BossPhase
    {
        public GuardianBattle(BossEncounter bossData, float hpUntil) : base(bossData)
        {
            phaseName = "PhaseName";
            phaseLength = 5;
            endHpPercentage = hpUntil;
            phaseType = PhaseType.HpBased;
            attackOrder = AttackOrder.Random;
            this.bossData = bossData as GuardianBossEncounter;

            rockfallAttack = new Rockfall(bossData, 2f);
            attacks = new List<BossAttack>() {
                new FistAttack(bossData, 1.5f),
                rockfallAttack,
                new ThrowAttack(bossData, 1f)
            };
            this.hpUntil = hpUntil;
        }

        public override void DebugStartPhase()
        {
            //AudioManager.PlayMusic(dummyBossData.GetComponent<AudioSource>(), 60);
        }

        protected override void PhaseUpdate()
        {
            rockfallAttack.UpdateVolley(rockfallAttack.volleyOne);
            rockfallAttack.UpdateVolley(rockfallAttack.volleyTwo);
        }

        public override void StartPhase()
        {
            base.StartPhase();
            bossData.bossHP.SetMinHpPercentage(hpUntil);
            bossData.PlayRunAnimation();
        }

        Rockfall rockfallAttack;
        GuardianBossEncounter bossData;
        private float hpUntil;
    }

    public class ConsumeCrystalMove : BossAttack
    {
        public ConsumeCrystalMove(BossEncounter bossData, float attackLength, GameObject crystalToMoveTo, bool allowInterruption = true, bool ended = false) 
            : base(bossData, attackLength, allowInterruption, ended)
        {
            this.bossData = bossData as GuardianBossEncounter;
            this.crystalToMoveTo = crystalToMoveTo;
        }

        protected override void AttackStart()
        {
            base.AttackStart();
            bossData.GetComponent<Align>().target = crystalToMoveTo;
            AudioManager.PlaySource("Trampling", bossData.tramplingSource);
        }

        protected override void AttackUpdate()
        {
            float distance = Vector2.Distance(crystalToMoveTo.transform.position, bossData.transform.position);
            if (distance < 6f)
            {
                AttackInterrupt();
            }
        }

        GuardianBossEncounter bossData;
        GameObject crystalToMoveTo;
    }

    public class ConsumeCrystal : BossAttack
    {
        public ConsumeCrystal(BossEncounter bossData, float attackLength, GameObject crystalToMoveTo, bool allowInterruption = true, bool ended = false) 
            : base(bossData, attackLength, allowInterruption, ended)
        {
            this.bossData = bossData as GuardianBossEncounter;
            this.crystalToMoveTo = crystalToMoveTo;
            aiAgent = bossData.GetComponent<AIAgent>();
        }

        protected override void AttackStart()
        {
            savedSpeed = aiAgent.moveSpeedMult;
            aiAgent.moveSpeedMult = 0;
            moveFrom = crystalToMoveTo.transform.position;
            mouthOffset = new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f), 0);
            bossData.PlayIdleAnimation();
            AudioManager.PauseSource(bossData.tramplingSource);
            AudioManager.Play(bossData.crystalConsumeAudio);
        }

        protected override void AttackUpdate()
        {
            float timeFraction = (attackLength - attackTimeLeft) / animationTime;
            crystalToMoveTo.transform.position = Vector3.Lerp(moveFrom, bossData.guardianMouth.position + mouthOffset, timeFraction);
            crystalToMoveTo.transform.localScale = Vector3.Lerp(Vector3.one, lowestScale, timeFraction);
        }

        protected override void AttackEnd()
        {
            crystalToMoveTo.transform.SetParent(bossData.transform);
            aiAgent.moveSpeedMult = savedSpeed;
            bossData.GetComponent<Align>().target = GameObject.FindGameObjectWithTag("Player");
            bossData.PlayRunAnimation();
        }

        private float animationTime = 1f;
        private float savedSpeed;
        private Vector3 lowestScale = new Vector3(0.00f, 0.00f, 0.00f);
        private Vector3 mouthOffset;
        private AIAgent aiAgent;
        private Vector3 moveFrom;
        GuardianBossEncounter bossData;
        GameObject crystalToMoveTo;
    }

    public class ConsumeCrystalPhase : BossPhase
    {
        public ConsumeCrystalPhase(BossEncounter bossData, float hpUntil, GameObject crystalToMoveTo) : base(bossData)
        {
            phaseName = "PhaseName";
            phaseLength = 5;
            endHpPercentage = hpUntil;
            phaseType = PhaseType.AttackBased;
            attackOrder = AttackOrder.Sequence;
            this.bossData = bossData as GuardianBossEncounter;
            this.hpUntil = hpUntil;
            attacks = new List<BossAttack>() {
                new ConsumeCrystalMove(bossData, 10f, this.bossData.roomCenter),
                new ConsumeCrystalMove(bossData, 10f, crystalToMoveTo),
                new ConsumeCrystal(bossData, 1.5f, crystalToMoveTo)
            };
            this.crystalToMoveTo = crystalToMoveTo;
        }

        public override void StartPhase()
        {
            base.StartPhase();
            bossData.bossHP.SetMinHpPercentage(hpUntil);
            bossData.PlayRunAnimation();
        }

        protected override void EndPhase()
        {
            base.EndPhase();
            if (crystalToMoveTo.name.ToLower().StartsWith("emerald"))
            {
                bossData.emeraldActive = true;
            }
            else if (crystalToMoveTo.name.ToLower().StartsWith("lava"))
            {
                bossData.lavaTrail.SetActive(true);
                bossData.GetComponent<AIAgent>().maxSpeed *= 1.2f;
            }
            else if (crystalToMoveTo.name.ToLower().StartsWith("onyx"))
            {
                bossData.onyxActive = true;
            }
            else if (crystalToMoveTo.name.ToLower().StartsWith("sapphire"))
            {
                bossData.sapphireShardsContainer.SetActive(true);
            }
            else if (crystalToMoveTo.name.ToLower().StartsWith("amethyst"))
            {
                bossData.amethystContainer.SetActive(true);
            }
        }

        GuardianBossEncounter bossData;
        private float hpUntil;
        private GameObject crystalToMoveTo;
    }

    public static int[] GetUniqueRandomArray(int min, int max, int count)
    {
        int[] result = new int[count];
        List<int> numbersInOrder = new List<int>();
        for (var x = min; x < max; x++)
        {
            numbersInOrder.Add(x);
        }
        for (var x = 0; x < count; x++)
        {
            var randomIndex = Random.Range(0, numbersInOrder.Count);
            result[x] = numbersInOrder[randomIndex];
            numbersInOrder.RemoveAt(randomIndex);
        }

        return result;
    }

    protected override void Start()
    {
        cameraShaker = GetComponent<ShakeCameraExternal>();
        encounterStarted = true;

        player = GameObject.FindGameObjectWithTag("Player").transform;

        Attack[] attacks = GetComponents<Attack>();
        rockThrowAttack = attacks[0];
        upgradedRockThrowAttack = attacks[1];

        GameObject[] crystals = new GameObject[] { lavaCrystal, onyxCrystal, sapphireCrystal, emeraldCrystal, amethystCrystal };
        int[] crystalSequence = GetUniqueRandomArray(0, crystals.Length, crystals.Length);

        float[] hpLimits = new float[] { 0.9f, 0.7f, 0.45f, 0.15f };
        bossPhases = new List<BossPhase>() {
            //new GuardianBattle(this, hpLimits[0]),
            //new ConsumeCrystalPhase(this, hpLimits[0], sapphireCrystal),
            //new GuardianBattle(this, hpLimits[1]),
            //new ConsumeCrystalPhase(this, hpLimits[1], lavaCrystal),
            //new GuardianBattle(this, hpLimits[2]),
            //new ConsumeCrystalPhase(this, hpLimits[2], onyxCrystal),
            //new GuardianBattle(this, hpLimits[3]),
            //new ConsumeCrystalPhase(this, hpLimits[3], emeraldCrystal),

            new GuardianBattle(this, hpLimits[0]),
            new ConsumeCrystalPhase(this, hpLimits[0], crystals[crystalSequence[0]]),
            new GuardianBattle(this, hpLimits[1]),
            new ConsumeCrystalPhase(this, hpLimits[1], crystals[crystalSequence[1]]),
            new GuardianBattle(this, hpLimits[2]),
            new ConsumeCrystalPhase(this, hpLimits[2], crystals[crystalSequence[2]]),
            new GuardianBattle(this, hpLimits[3]),
            new ConsumeCrystalPhase(this, hpLimits[3], crystals[crystalSequence[3]]),

            new GuardianBattle(this, 0),
        };
        base.Start();
    }

    private void PlayRunAnimation(bool walking = true)
    {
        bodyAnimator.SetBool("Walking", walking);
        crystalAnimator.SetBool("Walking", walking);
        headAnimator.SetBool("Walking", walking);
    }

    private void PlayIdleAnimation()
    {
        PlayRunAnimation(false);
    }

    protected override void EncounterUpdate()
    {
        base.EncounterUpdate();
        if (emeraldActive)
        {
            if (emeraldActiveTimestamp == 0)
            {
                emeraldActiveTimestamp = Time.time;
            }
            if (Time.time - emeraldActiveTimestamp < 1f)
            {
                for (int i = 0; i < emeraldSpikeContainer.transform.childCount; i++)
                {
                    var spike = emeraldSpikeContainer.transform.GetChild(i);
                    spike.Translate(spike.up * 2 * Time.deltaTime, Space.World);
                }
            }
        }
    }

    protected override void EncounterSuccess()
    {
        bossHP.Damage(null, 999999, ignoreInvulurability: true);
        base.EncounterSuccess();
        StartCoroutine(EndGame());
    }

    public IEnumerator EndGame()
    {
        GetComponent<ShakeCameraExternal>().ShakeCamera(2.75f, 2.15f);
        yield return new WaitForSeconds(2f);
        Metrics.OnWin();
        RelodScene.OnSceneChange?.Invoke();
        SceneLoading.CompleteEpisode(0);
    }

    private float emeraldActiveTimestamp = 0;
}
