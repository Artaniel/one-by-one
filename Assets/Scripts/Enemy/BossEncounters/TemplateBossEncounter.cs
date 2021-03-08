using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Duplicate this file with CTRL+D
// DON'T overwrite
public class TemplateBossEncounter : BossEncounter
{
    const float SET_VALUE = 2f;
    const float VALUE_FROM_RANGE_0_1 = 0.5f;

    public class TemplateAttack : BossAttack
    {
        public TemplateAttack(BossEncounter bossData, float attackLength, bool allowInterruption = true, bool ended = false) 
            : base(bossData, attackLength, allowInterruption, ended) {
            this.bossData = bossData as TemplateBossEncounter;
        }

        protected override void AttackStart()
        {
            
        }
        
        TemplateBossEncounter bossData;
    }

    public class TemplatePhase : BossPhase
    {
        public TemplatePhase(BossEncounter bossData, float hpUntil) : base(bossData)
        {
            phaseName = "DummyEnrage";
            phaseLength = SET_VALUE;
            endHpPercentage = VALUE_FROM_RANGE_0_1;
            phaseType = PhaseType.HpBased;
            attackOrder = AttackOrder.RandomRepeatable;
            this.bossData = bossData as TemplateBossEncounter;
            attacks = new List<BossAttack>() {
                new TemplateAttack(bossData, 2f),
            };
        }

        public override void DebugStartPhase()
        {
            AudioManager.PlayMusic(bossData.GetComponent<AudioSource>(), 0);
        }

        public override void StartPhase()
        {
            base.StartPhase();
            // for hp based phases
            bossData.bossHP.SetMinHpPercentage(VALUE_FROM_RANGE_0_1);
        }

        TemplateBossEncounter bossData;
    }

    protected override void Start()
    {
        encounterStarted = true;
        bossPhases = new List<BossPhase>() {
            new TemplatePhase(this, VALUE_FROM_RANGE_0_1),
        };
        base.Start();
    }

    protected override void EncounterSuccess()
    {
        base.EncounterSuccess();
    }
}
