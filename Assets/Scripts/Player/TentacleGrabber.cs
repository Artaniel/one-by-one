using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleGrabber : MonoBehaviour
{
    bool occupied = false;
    public bool broken { get; private set; } = false;
    private GameObject victim;
    private MonsterLife victimLife;
    public CircleCollider2D wallScanner;
    private float damage;

    private void OnEnable()
    {
        InitializeGrabber();
    }

    private void OnTriggerEnter2D(UnityEngine.Collider2D coll)
    {
        if (!occupied && coll.CompareTag("EnemyCollider"))
        {
            wallScanner.enabled = true;
            victim = coll.transform.parent.gameObject;
            victimLife = victim.GetComponent<MonsterLife>();
            victimLife.Damage(gameObject, damage: damage / 2f, ignoreSourceTime: 0.25f);
            occupied = true;
            timeHit = Time.time;
        }
        if (coll.CompareTag("Environment"))
        {
            if (Time.time - timeEnabled > 0.25f)
            {
                broken = true;
                if (occupied && Time.time - timeHit > 0.25f)
                {
                    victimLife.Damage(gameObject, damage: damage / 2f, ignoreSourceTime: 0.25f);
                }
            }
        }
    }

    private void Update()
    {
        if (occupied)
        {
            if (victimLife.HP <= 0)
            {
                InitializeGrabber();
            }
            else
            {
                victim.transform.position = edge.position;
            }
        }
    }

    public void InitializeGrabber()
    {
        victim = null;
        broken = false;
        occupied = false;
        wallScanner.enabled = false;
        timeEnabled = Time.time;
        edge = transform.GetChild(0);
    }

    public void SetDamage(float damage) => this.damage = damage;

    private Transform edge;

    private Vector3 grabPosition;
    private float timeHit;
    private float timeEnabled;
}
