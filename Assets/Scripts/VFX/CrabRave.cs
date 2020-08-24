using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabRave : MonoBehaviour
{
    void Start()
    {
        holder = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        var vectorToPlayer = holder.position - transform.position;
        var distance = vectorToPlayer.magnitude;
        if (distance > moveDistanceThreshold)
        {
            var newPosition = transform.position + vectorToPlayer.normalized * distance * Time.deltaTime;
            transform.position = newPosition;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Vector3.forward, vectorToPlayer), 1);
        }

        if (Vector3.Distance(transform.position, lastPosition) > animationThreshold)
        {
            animator.Play("CrabRave");
        }
        else
        {
            animator.Play("CrabStop");
        }
        lastPosition = transform.position;
    }

    private Transform holder;
    private Animator animator;
    private Vector3 lastPosition = Vector3.zero;
    private float animationThreshold = 0.00001f;
    private float moveDistanceThreshold = 2f;
}
