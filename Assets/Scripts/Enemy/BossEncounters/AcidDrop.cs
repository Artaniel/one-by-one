using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidDrop : MonoBehaviour
{
    public Vector3 targetPosition;
    public float velocity;
    private bool isMoving = false;

    private void OnEnable()
    {
        animator = GetComponentInChildren<Animator>();
        dynamicGrow = GetComponentInChildren<ColliderDynamicGrow>();
        animator.Play("Empty");
    }

    private void Update()
    {
        if (isMoving && !Pause.Paused) {
            if (Vector3.Distance(transform.position, targetPosition) > velocity * Time.deltaTime)
            {
                transform.position += (targetPosition - transform.position).normalized * velocity * Time.deltaTime;
                animator.Play("Empty");
            }
            else
            {
                transform.position = targetPosition;
                isMoving = false;
                animator.Play("ExplosiveAcid");
                dynamicGrow.shouldGrow = true;
            }
        }
    }

    public void StartMove() {
        isMoving = true;
    }

    private Animator animator;
    private ColliderDynamicGrow dynamicGrow;
}
