using UnityEngine;
using System.Linq;

public abstract class Align : EnemyBehavior
{
    public float timeToTarget = 0.1f;
    public bool rotateAtStart = true;

    protected float maxBypassRaycastDistance = 3f;
    [SerializeField, Header("If bypass is needed choose 70. It is good")]
    protected float bypassAngleAccumulationSpeed = 0; // 70 is a good normal value
    protected float bypassAngleAccumulator = 50;

    protected virtual void Start()
    {
        if (rotateAtStart) RotateInstantlyTowardsTarget();
    }

    public virtual float GetRotation(float targetOrientation = 0)
    {
        if (isActive)
        {
            float desiredRotation = targetOrientation - agent.orientation;
            if (bypassAngleAccumulationSpeed != 0) desiredRotation += AccumulateBypassAngle();
            desiredRotation = MapToRange(desiredRotation);

            this.targetOrientation = targetOrientation;
            return desiredRotation / timeToTarget;
        }
        else
        {
            return 0;
        }
    }

    private float AccumulateBypassAngle()
    {
        var direction = target.transform.position - transform.position;

        var hits = RaycastHits(direction, maxBypassRaycastDistance);
        hits = (from t in hits
                where (t.transform.tag == "EnemyCollider" && t.transform.parent != transform) || t.transform.tag == "Environment" || t.transform.tag == "Player"
                select t).ToArray();
        // var status = hits.Length != 0 && hits[0].transform.gameObject.tag != "Player" ? "Found wall" : "Wall not found, " + hits.Length;
        if (hits.Length != 0 && hits[0].transform.gameObject.tag != "Player")
        {
            bypassAngleAccumulator += Mathf.Sign(bypassAngleAccumulator + targetOrientation / 2) * bypassAngleAccumulationSpeed * Time.deltaTime;
        }
        else
        {
            bypassAngleAccumulator -= Mathf.Sign(bypassAngleAccumulator) * bypassAngleAccumulationSpeed * Time.deltaTime * 2f;
        }
        return(bypassAngleAccumulator);
        // print(status + ": " + bypassAngleAccumulator + " -> " + targetOrientation);
    }

    private RaycastHit2D[] RaycastHits(Vector2 direction, float distance)
    {
        //Debug.DrawLine(transform.position, direction1.normalized);
        return Physics2D.RaycastAll(transform.position, new Vector2(transform.up.x, transform.up.y) * 0.5f + direction.normalized, distance);
    }

    protected float targetOrientation;
}
