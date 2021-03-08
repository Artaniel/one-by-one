using UnityEngine;

public class LizardBooster : Attack
{
    [SerializeField]
    private float boostedSpeed = 2f;

    [SerializeField]
    private float boostTime = 2.0f;

    [SerializeField]
    private bool stopOnHit = true;

    protected override void Start()
    {
        base.Start();
        baseSpeed = agent.moveSpeedMult;
        boostTimeLeft = 0.0f;

        if (stopOnHit)
        {
            var monsterLife = GetComponent<MonsterLife>();
            monsterLife.OnThisAbsorb.AddListener(StopBoost);
            monsterLife.OnThisHit.AddListener(StopBoost);
        }
    }

    protected override void DoAttack()
    {
        var audio = GetComponent<AudioSource>();
        AudioManager.Play("LizardRun", audio);
            
        boostTimeLeft = boostTime;
        agent.moveSpeedMult *= boostedSpeed;
    }

    public override void CalledUpdate()
    {
        base.CalledUpdate();
        boostTimeLeft = Mathf.Max(boostTimeLeft - Time.deltaTime, 0);

        if (boostTimeLeft <= 0)
        {
            agent.moveSpeedMult = baseSpeed;
        }
    }

    protected void StopBoost()
    {
        Reload();
        boostTimeLeft = 0;
    }

    private float baseSpeed;
    private float boostTimeLeft;
}
