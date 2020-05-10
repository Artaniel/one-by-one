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
        if (collision.gameObject == player) {
            //VFX/SFX?
            Open();
            Destroy(gameObject);
        }
    }
}
