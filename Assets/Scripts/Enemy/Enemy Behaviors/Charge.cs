using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MoveForward))]
public class Charge : TimedAttack
{
    [SerializeField] private MoveForward simpleMoveForward = null;
    [SerializeField] private MoveForward otherMoveForward = null;

    protected override void Awake()
    {
        base.Awake();
        aiAgent = GetComponent<AIAgent>();
        //moveBehaviours = GetComponents<MoveBehaviour>();
        simpleMoveForward.speedMult = 0;
    }

    protected override void AttackAnimation()
    {
        StartCoroutine(ChargeCharge());
    }

    protected override void CompleteAttack()
    {
        simpleMoveForward.speedMult = 0;
        otherMoveForward.speedMult = 1;
        aiAgent.maxRotation = savedRotation;
    }

    protected IEnumerator ChargeCharge()
    {
        yield return new WaitForSeconds(0.5f);
        simpleMoveForward.speedMult = 2;
        otherMoveForward.speedMult = 0;
        savedRotation = aiAgent.maxRotation;
        aiAgent.maxRotation = 0;
    }

    private AIAgent aiAgent;
    
    //private MoveBehaviour[] moveBehaviours;
    private float savedRotation;
}
