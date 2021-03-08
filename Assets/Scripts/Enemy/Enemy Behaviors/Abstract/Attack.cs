using UnityEngine;

public abstract class Attack : EnemyBehavior
{
    [SerializeField, Header("Attack Block")]
    protected Vector2 cooldownRange = new Vector2(1f, 1f);
    [HideInInspector] public float attackSpeedModifier = 1f;
    [SerializeField] protected AudioClip attackSound = null;

    protected override void Awake()
    {
        base.Awake();
        Reload();
    }

    public override void CalledUpdate()
    {
        base.CalledUpdate();
        if (isActive)
        {
            cooldownLeft = Mathf.Max(cooldownLeft - Time.deltaTime, 0);
            if (cooldownLeft <= 0)
            {
                Reload();
                DoAttack();
            }
        }
    }

    public void ForceAttack()
    {
        Reload();
        DoAttack();
    }

    public void Reload()
    {
        cooldownLeft = Random.Range(cooldownRange.x, cooldownRange.y) / attackSpeedModifier;
    }

    public void SetCooldownRange(Vector2 newCooldownRange)
    {
        cooldownRange = newCooldownRange;
    }

    protected abstract void DoAttack();

    protected float cooldownLeft;
}