using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailMove : MoveBehaviour
{
    [SerializeField] private Transform[] rail;
    private enum RailMoveMode {endToEnd, loop}
    [SerializeField] private RailMoveMode railMode = RailMoveMode.endToEnd;
    private enum MoveStage {wait, go}
    private MoveStage moveStage = MoveStage.wait;
    [SerializeField] private float waitTime = 1f;
    private float timer = 1f;
    private int nextPointIndex = 0;
    private bool goingBack = false;

    protected override void Awake()
    {
        base.Awake();
        if (rail != null) {
            transform.position = rail[0].position;
        }
    }

    public override Vector2 Move()
    {
        if (!isActive) return Vector2.zero;
        if (moveStage == MoveStage.wait) {
            timer -= Time.fixedDeltaTime;
            if (timer <= 0) {
                ChooseNextPoint();
                moveStage = MoveStage.go;
            }
            return Vector2.zero;
        }
        else
        {
            Vector2 target = rail[nextPointIndex].position - transform.position;
            if (target.magnitude <= agent.maxSpeed * Time.deltaTime)
            {
                moveStage = MoveStage.wait;
                timer = waitTime;
                transform.position = rail[nextPointIndex].position; // override move to set in exact point
                return Vector2.zero;
            }
            else {
                return (target.normalized * agent.maxSpeed);
            }
        }
    }

    private void ChooseNextPoint() {
        if (railMode == RailMoveMode.endToEnd)
        {
            if (!goingBack)
            {
                nextPointIndex++;
                if (nextPointIndex >= rail.Length)
                { // reached end and turned back
                    goingBack = true;
                    nextPointIndex = rail.Length - 1;
                }
            }
            else {
                nextPointIndex--;
                if (nextPointIndex < 0)
                { // reached start and turned forward again
                    goingBack = false;
                    nextPointIndex = 1;
                }
            }
        } else {
            nextPointIndex++;
            nextPointIndex %= rail.Length;//reached end and move to first point
        }
    }
}
