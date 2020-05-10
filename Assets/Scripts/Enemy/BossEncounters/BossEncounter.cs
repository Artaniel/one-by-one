using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossEncounter : MonoBehaviour
{
    public int startFromPhase = 0;

    [System.Serializable]
    public class BossAttack
    {
        protected float attackLength = 5f;
        public bool allowInterruption = true;
        public bool ended = false;

        public BossAttack(BossEncounter bossData, float attackLength, bool allowInterruption = true, bool ended = false)
        {
            this.attackLength = attackLength;
            this.allowInterruption = allowInterruption;
            this.ended = ended;
            attackTimeLeft = attackLength;
            baseBossData = bossData;
        }

        public void BaseAttackStart()
        {
            ended = false;
            attackTimeLeft = attackLength;
            AttackStart();
        }

        protected virtual void AttackStart() { }

        public void BaseAttackUpdate()
        {
            attackTimeLeft -= Time.deltaTime;
            //print(attackTimeLeft);
            if (attackTimeLeft <= 0 && !ended)
            {
                BaseAttackEnd();
            }
            AttackUpdate();
        }

        protected virtual void AttackUpdate() { }

        protected virtual void AttackEnd() { }

        public void BaseAttackEnd()
        {
            if (ended) return;
            ended = true;
            AttackEnd();
        }

        public virtual void AttackInterrupt()
        {
            AttackEnd();
        }

        protected float attackTimeLeft = 100f;
        protected BossEncounter baseBossData;
    }
    
    [System.Serializable]
    public class BossPhase
    {
        public enum PhaseType { Unknown, TimeBased, AttackBased, HpBased, TimeOrHpBased, Trigger }
        public enum AttackOrder { Random, RandomRepeatable, Sequence, SequenceWithLoop }

        protected string phaseName = "Unnamed Phase";
        protected float endHpPercentage = -1;
        protected float phaseLength = -1;
        protected PhaseType phaseType = PhaseType.TimeBased;
        protected AttackOrder attackOrder = AttackOrder.Sequence;
        protected float musicTimestamp = 0;
        protected List<BossAttack> attacks = null;
        protected bool phaseEnded = false;

        protected int currentAttackNumber = -1;

        public void PhaseBaseUpdate() {
            if (phaseEnded) return;

            PhaseUpdate();
            phaseTimer += Time.deltaTime;

            attacks[currentAttackNumber].BaseAttackUpdate();
            if (attacks[currentAttackNumber].ended)
            {
                StartNextAttack();
            }

            phaseEnded = CheckEndPhase();
        }

        public virtual void StartPhase() => StartNextAttack();

        protected virtual void PhaseUpdate() { }

        private void StartNextAttack()
        {
            int nextAttackNumber = -1;
            switch (attackOrder)
            {
                case AttackOrder.Random:
                    var exitCounter = 0;
                    nextAttackNumber = Random.Range(0, attacks.Count);
                    while (nextAttackNumber == currentAttackNumber && exitCounter != 50)
                    {
                        nextAttackNumber = Random.Range(0, attacks.Count);
                        exitCounter++;
                    }
                    break;
                case AttackOrder.RandomRepeatable:
                    nextAttackNumber = Random.Range(0, attacks.Count);
                    break;
                case AttackOrder.Sequence:
                    nextAttackNumber = currentAttackNumber + 1;
                    break;
                case AttackOrder.SequenceWithLoop:
                    nextAttackNumber = (currentAttackNumber + 1) % attacks.Count;
                    break;
                default:
                    break;
            }
            if (currentAttackNumber != -1)
            {
                attacks[currentAttackNumber].BaseAttackEnd();
            }
            
            if (phaseEnded || nextAttackNumber >= attacks.Count) return;

            currentAttackNumber = nextAttackNumber;
            
            attacks[currentAttackNumber].BaseAttackStart();

            OnNextAttackStart();
        }

        protected virtual void OnNextAttackStart() { }

        public bool CheckEndPhase()
        {
            switch (phaseType)
            {
                case PhaseType.Unknown:
                    Debug.LogWarning("Phase type not set");
                    return true;
                case PhaseType.TimeBased:
                    return phaseTimer >= phaseLength;
                case PhaseType.AttackBased:
                    return attacks[currentAttackNumber].ended && currentAttackNumber >= attacks.Count - 1;
                case PhaseType.HpBased:
                    return bossData.BossHealthPercentage() <= endHpPercentage;
                case PhaseType.TimeOrHpBased:
                    return phaseTimer >= phaseLength || bossData.BossHealthPercentage() >= endHpPercentage;
                case PhaseType.Trigger:
                    return phaseEnded;
                default:
                    Debug.LogWarning("Phase type not set");
                    return true;
            }
        }

        public bool HasPhaseEnded() => phaseEnded;

        public void BaseEndPhase() {
            attacks[currentAttackNumber].BaseAttackEnd();
            EndPhase();
        }

        protected virtual void EndPhase() { }

        public virtual void DebugStartPhase() {
            StartPhase();
        }

        protected float phaseTimer = 0;
        private BossEncounter bossData;

        public BossPhase(BossEncounter bossData) => this.bossData = bossData;
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        bossHP = GetComponent<MonsterLife>();
        if (startFromPhase == 0)
        {
            StartEncounter();
        }
        else
        {
            phaseID = startFromPhase;
            currentPhase = bossPhases[phaseID];
            currentPhase.DebugStartPhase();
        }
    }

    protected virtual void StartEncounter()
    {
        NextPhaseOrFinish();
    } 

    // Update is called once per frame
    void Update()
    {
        if (encounterOver || !encounterStarted) return;

        if (currentPhase.HasPhaseEnded())
            NextPhaseOrFinish();
        else
            bossPhases[phaseID].PhaseBaseUpdate();
        
        EncounterUpdate();
    }

    private void NextPhaseOrFinish()
    {
        if (phaseID != -1)
        {
            currentPhase.BaseEndPhase();
        }
        phaseID++;
        if (phaseID == bossPhases.Count)
        {
            encounterOver = true;
            EncounterSuccess();
        }
        else
        {
            Debug.Log($"Starting phase {phaseID}");
            currentPhase = bossPhases[phaseID];
            currentPhase.StartPhase();
        }
    }

    public float BossHealthPercentage()
    {
        return bossHP.HP / bossHP.maxHP;
    }

    protected virtual void EncounterUpdate() { }
    protected virtual void EncounterSuccess() { }

    protected List<BossPhase> bossPhases = new List<BossPhase>() { };
    protected BossPhase currentPhase;
    protected int phaseID = -1;
    protected bool encounterStarted = false;
    protected bool encounterOver = false;
    public MonsterLife bossHP = null;
}
