using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourWayAlign : Align
{
    enum Direction { North, East, South, West }
    private Direction direction;

    public float timeToScan = 1.25f;
    private float timeToScanLeft;

    protected override void Awake()
    {
        base.Awake();
        timeToScanLeft = timeToScan;
    }

    public override float GetRotation(float targetOrientation = 0)
    {
        if (timeToScanLeft < 0)
        {
            timeToScanLeft = timeToScan;
            Direction firstDirection = target.transform.position.y > transform.position.y ? Direction.North : Direction.South;
            Direction secondDirection = target.transform.position.x > transform.position.x ? Direction.East : Direction.West;
            if (firstDirection == direction)
                direction = secondDirection;
            else if (secondDirection == direction)
                direction = firstDirection;
            else if (
                   (direction == Direction.North && firstDirection == Direction.South)
                || (direction == Direction.South && firstDirection == Direction.North))
                direction = secondDirection;
            else if (
                   (direction == Direction.East && secondDirection == Direction.West)
                || (direction == Direction.West && secondDirection == Direction.East))
                direction = firstDirection;
            else
                direction = Random.Range(0, 1f) > 0.5f ? firstDirection : secondDirection;
        }
        timeToScanLeft -= Time.deltaTime;

        switch (direction)
        {
            case Direction.North:
                targetOrientation = 0;
                break;
            case Direction.East:
                targetOrientation = 90;
                break;
            case Direction.South:
                targetOrientation = 180;
                break;
            case Direction.West:
                targetOrientation = 270;
                break;
            default:
                break;
        }
        return base.GetRotation(targetOrientation);
    }
}
