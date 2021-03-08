using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardianBossEncounter : BossEncounter
{
    public GameObject attackWave;
    public ZoneScript rockFallZone;
    public GameObject rockFalling;
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

    [HideInInspector]
    public ShakeCameraExternal cameraShaker;

    public class Rockfall : BossAttack
    {
        public Rockfall(BossEncounter bossData, float attackLength, bool allowInterruption = true, bool ended = false)
            : base(bossData, attackLength, allowInterruption, ended)
        {
            this.bossData = bossData as GuardianBossEncounter;
            this.bossData.rockFallZone.UseZone();
        }

        protected override void AttackStart()
        {
            RockFall(50);
            if (bossData.onyxActive) bossData.StartCoroutine(DelayedRockFall());
        }

        private void RockFall(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var rock = PoolManager.GetPool(bossData.rockFalling, bossData.rockFallZone.RandomZonePosition3(), Quaternion.identity);
                PoolManager.ReturnToPool(rock, 1.1f);
            }
            bossData.cameraShaker.ShakeCamera();
        }

        private IEnumerator DelayedRockFall()
        {
            yield return new WaitForSeconds(0.5f);
            RockFall(40);
        }

        GuardianBossEncounter bossData;
    }

    public class FistAttack : BossAttack
    {
        public FistAttack(BossEncounter bossData, float attackLength, bool allowInterruption = true, bool ended = false) 
            : base(bossData, attackLength, allowInterruption, ended) {
            this.bossData = bossData as GuardianBossEncounter;
            aiAgent = this.bossData.GetComponent<AIAgent>();
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
        }

        private IEnumerator DelayedStart()
        {
            yield return new WaitForSeconds(waitBeforeAttack - 0.1f); // Because otherwise update happens

            bossData.GetComponent<AIAgent>().moveSpeedMult /= 100 * 2f;
            attackWaveInstance = PoolManager.GetPool(bossData.attackWave, bossData.transform.position, bossData.transform.rotation);
            attackWaveInstance.transform.position = bossData.transform.position + bossData.transform.up * 2f;
            attackWaveInstance.transform.Rotate(0, 0, -rotateSpeed * 0.5f * realAttackDuration);
            attackWaveInstance.SetActive(true);

            attackWaveInstance.GetComponentInChildren<TrailRenderer>().Clear();
        }

        protected override void AttackUpdate()
        {
            if (attackLength - attackTimeLeft < waitBeforeAttack) return;

            if (attackTimeLeft > waitAfterAttack)
            {
                attackWaveInstance.transform.Rotate(0, 0, rotateSpeed * Time.deltaTime, Space.Self);
            }
        }

        protected override void AttackEnd()
        {
            bossData.StopCoroutine(delayedStartCoroutine);
            if (attackWaveInstance)
            {
                PoolManager.ReturnToPool(attackWaveInstance);
            }
            aiAgent.moveSpeedMult = savedSpeed;
            aiAgent.maxRotation = savedRotation;
        }

        GuardianBossEncounter bossData;
        float rotateSpeed;
        float savedRotation;
        float waitAfterAttack = 0.5f;
        float waitBeforeAttack = 0.5f;
        float realAttackDuration;
        GameObject attackWaveInstance;
        AIAgent aiAgent;
        Coroutine delayedStartCoroutine;
        private float savedSpeed;
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
            attacks = new List<BossAttack>() {
                new FistAttack(bossData, 1.5f),
                new Rockfall(bossData, 1.5f),
                new ThrowAttack(bossData, 1f)
            };
            this.hpUntil = hpUntil;
        }

        public override void DebugStartPhase()
        {
            //AudioManager.PlayMusic(dummyBossData.GetComponent<AudioSource>(), 60);
        }

        public override void StartPhase()
        {
            base.StartPhase();
            bossData.bossHP.SetMinHpPercentage(0);
        }

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
        }

        private float animationTime = 1f;
        private float savedSpeed;
        private Vector3 lowestScale = new Vector3(0.05f, 0.05f, 0.05f);
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
                    spike.Translate(-spike.up * 2 * Time.deltaTime, Space.World);
                }
            }
        }
    }

    protected override void EncounterSuccess()
    {
        bossHP.Damage(null, 999999, ignoreInvulurability: true);
        base.EncounterSuccess();
    }

    private float emeraldActiveTimestamp = 0;
}
