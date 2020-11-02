using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MosquitoMove : MoveForward
{
    [Header("Mosquito-Specific values")]
    public float randomPointDistance = 4f;
    public float pauseAndShootTime = 1f;

    protected override void Awake()
    {
        base.Awake();
        RecalculatePoint();
        attacks = GetComponents<Attack>();

        if (Labirint.instance == null)
        {
            arena = GameObject.FindGameObjectWithTag("GameController")
            .GetComponent<ArenaEnemySpawner>();
        }
        else
        {
            arena = Labirint.GetCurrentRoom().GetComponent<ArenaEnemySpawner>();
        }

        var coll = GetComponentInChildren<BoxCollider2D>();
        monsterSize = coll.size.x + coll.size.y / 2;
    }

    public override Vector2 Move()
    {
        if (!isActive) return Vector2.zero;
        timeToMaxSpeedAmp += Time.deltaTime;

        var distanceToPointClamped = Mathf.Clamp01(Vector3.Distance(movePosition, transform.position));
        if (distanceToPointClamped < 2f && attackAndWait == null)
            attackAndWait = StartCoroutine(AttackAndWait());

        Vector2 movement = (movePosition - transform.position).normalized;
        return movement * speedMult * distanceToPointClamped * (agent.maxSpeed * Mathf.InverseLerp(0, timeToMaxSpeed, timeToMaxSpeedAmp));
    }

    private void RecalculatePoint()
    {
        timeToMaxSpeedAmp = 0;
        Vector3 playerPosition = target.transform.position;

        int counter = 0;
        while (counter < 8)
        {
            counter++;
            Vector3 nextMovePosition = Random.insideUnitCircle.normalized * randomPointDistance;
            Debug.DrawRay(target.transform.position, nextMovePosition, Color.green, 1);

            bool inbounds = false;
            if (Labirint.instance && Labirint.currentRoom)
            {
                inbounds = Labirint.currentRoom.RectIsInbounds(target.transform.position.x + nextMovePosition.x, target.transform.position.y + nextMovePosition.y, 0, 0);
            }
            else if (arena)
            {
                inbounds = (arena.RoomBounds.x > Mathf.Abs(target.transform.position.x + nextMovePosition.x) &&
                arena.RoomBounds.y > Mathf.Abs(target.transform.position.y + nextMovePosition.y));
            }
            else inbounds = true;

            if (inbounds == false) continue;

            var vectorToNewPosition = (target.transform.position + nextMovePosition) - transform.position;
            var canDrawDirectLine = !(Physics2D.CircleCast(transform.position + (vectorToNewPosition.normalized * monsterSize), 
                monsterSize, vectorToNewPosition,
                vectorToNewPosition.magnitude, LayerMask.GetMask("Solid")));

            if (canDrawDirectLine)
            {
                movePosition = new Vector3(playerPosition.x, playerPosition.y) + nextMovePosition;
                break;
            }
        }
    }

    private IEnumerator AttackAndWait()
    {
        Attack();
        yield return new WaitForSeconds(pauseAndShootTime);
        RecalculatePoint();
        attackAndWait = null;
    }

    private void Attack()
    {
        foreach (var attack in attacks)
        {
            attack.ForceAttack();
        }
    }

    Coroutine attackAndWait;
    private Vector3 movePosition = Vector2.zero;
    private Attack[] attacks;
    private ArenaEnemySpawner arena;
    private float monsterSize;
}
