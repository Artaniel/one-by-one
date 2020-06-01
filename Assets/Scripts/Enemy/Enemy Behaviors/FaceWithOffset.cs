using UnityEngine;

public class FaceWithOffset : Align
{
    [SerializeField]
    private float offsetAngleMax = 40.0f;

    [SerializeField]
    private Vector2 cooldownRange = new Vector2(2f, 3f);

    protected override void Awake()
    {
        base.Awake();
        offsetAngle = Random.Range(-offsetAngleMax, offsetAngleMax);
        cooldownLeft = Random.Range(cooldownRange.x, cooldownRange.y);
    }

    public override void CalledUpdate()
    {
        cooldownLeft = Mathf.Max(cooldownLeft - Time.deltaTime, 0);
        if (cooldownLeft <= 0)
        {
            offsetAngle = Random.Range(-offsetAngleMax, offsetAngleMax);
            cooldownLeft = Random.Range(cooldownRange.x, cooldownRange.y);
        }
        base.CalledUpdate();
    }

    public override float GetRotation(float rotationAmp = 0)
    {
        float targetOrientation = 0;
        Vector2 direction = target.transform.position - transform.position;
        if (direction.magnitude > 0.0f)
        {
            targetOrientation = Mathf.Atan2(direction.x, direction.y);
            targetOrientation *= Mathf.Rad2Deg;
            targetOrientation += offsetAngle;
            targetOrientation %= 360.0f;
            if (targetOrientation < 0.0f)
            {
                targetOrientation += 360.0f;
            }
            base.targetOrientation = targetOrientation;
        }
        
        return base.GetRotation(targetOrientation + rotationAmp);
    }

    private float offsetAngle;
    private float cooldownLeft;
}
