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
        base.Awake();
        BoxCollider = GetComponentInChildren<BoxCollider2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        standardSpeed = agent.moveSpeedMult;
        audioSource = GetComponent<AudioSource>();
    }

    protected override void DoAttack()
    {
        audioSource.clip = attackSound;
        AudioManager.Play("GhostPhase", audioSource);

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
    private AudioSource audioSource;
}
