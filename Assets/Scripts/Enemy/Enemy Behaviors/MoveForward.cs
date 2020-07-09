using UnityEngine;

public class MoveForward : MoveBehaviour
{
    public float timeToMaxSpeedAmp = 0;
    public float timeToMaxSpeed = 0.33f;
    public float speedMult = 1f;

    public override Vector2 Move()
    {
        if (!isActive) return Vector2.zero;

        timeToMaxSpeedAmp += Time.deltaTime;
        Vector2 movement = transform.rotation * Vector3.up;
        return movement * speedMult * (agent.maxSpeed * Mathf.InverseLerp(0, timeToMaxSpeed, timeToMaxSpeedAmp));
    }
}