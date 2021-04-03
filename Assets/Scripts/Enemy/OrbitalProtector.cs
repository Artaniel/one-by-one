﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalProtector : EnemyMovement
{
    [SerializeField]
    private float SpinSpeed = 3;
    [SerializeField]
    private float Radius = 2;
    [SerializeField]
    private float FreeFlySpeed = 6;

    private GameObject host;
    private float angle;
    public void SetHost(GameObject host, float angle)
    {
        this.host = host;
        this.angle = angle;
    }

    // Update is called once per frame
    protected override void UpdateEnemy()
    {
        if (host != null)
        {
            var previousPosition = transform.position;
            angle += Time.deltaTime * SpinSpeed;
            var x = Mathf.Cos(angle) * Radius;
            var y = Mathf.Sin(angle) * Radius;
            transform.position = new Vector3(x, y, 0) + host.transform.position;
            direction = Vector3.Normalize(transform.position - previousPosition);
        }
        else
        {
            transform.Translate(direction * FreeFlySpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(UnityEngine.Collider2D coll)
    {
        if (coll.gameObject.tag == "Environment")
        {
            if (host == null) Destroy(gameObject);
        }
        else if (coll.gameObject.tag == "Player")
        {
            coll.gameObject.GetComponent<CharacterLife>().Damage();
        }
    }

    private Vector3 direction;
}
