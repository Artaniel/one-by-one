using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Container
{
    private GameObject player = null;

    protected override void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        base.Start();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!opened && collision.gameObject == player) {
            //VFX/SFX?
            opened = true;
            OpenAnimation();
            Open();
            //Destroy(gameObject);
        }
    }

    private void OpenAnimation()
    {
        var animator = GetComponentInChildren<Animator>();
        if (animator)
        {
            animator.Play("Chest-open");
        }
        if (!animator)
        {
            Destroy(gameObject);
        }
    }

    private bool opened = false;
}
