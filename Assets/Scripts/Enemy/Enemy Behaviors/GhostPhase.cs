using UnityEngine;

public class GhostPhase : Attack
{
    [SerializeField]
    private float GhostBoostSpeed = 7f;
    [SerializeField]
    private bool PacifistInBoost = true;
    [SerializeField]
    private float BoostTime = 2.5f;

    protected override void Awake()
    {
        BoxCollider = GetComponentInChildren<BoxCollider2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        cooldownLeft = cooldownLeft / 2;
        base.Awake();
    }

    protected void Start()
    {
        standardSpeed = agent.moveSpeedMult;
    }

    protected override void DoAttack()
    {
        var audio = GetComponent<AudioSource>();
        AudioManager.Play("Ghost", audio);

        BoxCollider.isTrigger = PacifistInBoost;
        agent.moveSpeedMult *= GhostBoostSpeed;
        var s = sprite.color;
        s.a = 0.5f;
        sprite.color = s;

        boostTimeLeft = BoostTime;
    }

    public override void CalledUpdate()
    {
        base.CalledUpdate();
        boostTimeLeft -= Time.deltaTime;
        if (boostTimeLeft <= 0)
        {
            BoxCollider.isTrigger = false;
            agent.moveSpeedMult = standardSpeed;
            var s = sprite.color;
            s.a = 1f;
            sprite.color = s;
        }
    }

    private float boostTimeLeft;
    private BoxCollider2D BoxCollider;
    private SpriteRenderer sprite;
    private float standardSpeed;
}
