using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabNotRave : MonoBehaviour
{
    Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        var vectorFromPlayer = transform.position - player.position;
        if (vectorFromPlayer.magnitude < 5)
        {
            transform.Translate(vectorFromPlayer.normalized * ((8f - vectorFromPlayer.magnitude) * Time.deltaTime), Space.World);
            animator.Play("CrabRave");
        }
        else
        {
            animator.Play("CrabStop");
        }
    }

    private Animator animator;
}
