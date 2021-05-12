using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotalMovement : MoveBehaviour
{
    public List<Transform> pivots;

    public override Vector2 Move()
    {
        Vector3 toMove = Vector2.zero;
        var pivotsCount = pivots.Count;
        var selfPosition = transform.position;
        foreach (var pivot in pivots)
        {
            toMove += (pivot.position - selfPosition);
        }
        return (toMove.magnitude > 1 ? toMove.normalized : toMove) * agent.maxSpeed;
    }
}
