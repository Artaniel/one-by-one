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

            this.targetOrientation = desiredRotation;
            return desiredRotation / timeToTarget;
        }
        else
        {
            return 0;
        }
    }

    protected float AccumulateBypassAngle()
    {
        Vector2 direction = target.transform.position - transform.position;
        var hits = RaycastHits(direction, 50);
        hits = (from t in hits
                where t.transform.tag == "Player" || t.transform.tag == "Environment"
                select t).ToArray();
        if (hits[0].transform.tag != "Player")
        {
            Debug.DrawRay(transform.position, direction * maxBypassRaycastDistance, Color.red);
            direction = ((direction.normalized * 0.5f) + (new Vector2(transform.up.x, transform.up.y))).normalized;
            Debug.DrawRay(transform.position, direction * maxBypassRaycastDistance, Color.cyan);

            hits = RaycastHits(direction, maxBypassRaycastDistance);
            hits = (from t in hits
                    where (t.transform.tag == "EnemyCollider" && t.transform.parent != transform) || t.transform.tag == "Environment"
                    select t).ToArray();
            // var status = hits.Length != 0 && hits[0].transform.gameObject.tag != "Player" ? "Found wall" : "Wall not found, " + hits.Length;
            if (hits.Length != 0)
            {
                float localX = transform.InverseTransformVector(Vector2.Reflect(direction, hits[0].normal)).x;
                if (Mathf.Abs(localX) < 0.1f)
                {
                    bypassAngleAccumulator += bypassAngleAccumulationSpeed * 4f * Time.deltaTime;
                }
                else if (bypassAngleAccumulator < 2)
                {
                    bypassAngleAccumulator += Mathf.Sign(localX) * bypassAngleAccumulationSpeed * 4f * Time.deltaTime;
                }
                else
                {
                    bypassAngleAccumulator += Mathf.Sign(bypassAngleAccumulator) * bypassAngleAccumulationSpeed * 4f * Time.deltaTime;
                }
            }
            else
            {
                bypassAngleAccumulator -= Mathf.Sign(bypassAngleAccumulator) * bypassAngleAccumulationSpeed * Time.deltaTime * 1f;
            }
        }
        else
        {
            Debug.DrawRay(transform.position, direction * maxBypassRaycastDistance, Color.green);
            bypassAngleAccumulator = 0;
        }
        
        return(MapToRange(bypassAngleAccumulator));
        // print(status + ": " + bypassAngleAccumulator + " -> " + targetOrientation);
    }

    private RaycastHit2D[] RaycastHits(Vector2 direction, float distance)
    {
        //Debug.DrawLine(transform.position, direction1.normalized);
        return Physics2D.RaycastAll(transform.position, direction, distance);
    }

    protected float targetOrientation;
}
