using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetonateOnDeath : MonoBehaviour
{
    [SerializeField] private float blastRadius = 1f;
    [SerializeField] private float detonateDelay = 0.5f;
    private bool detonateOnTimer = false;
    private float timer = 0f;


    private void Awake()
    {
        MonsterLife.OnEnemyDead.AddListener(StartDetonation);
    }

    private void Blast()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, blastRadius, Vector3.forward, 0f);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.tag == "Player")
            {
                hit.transform.GetComponent<CharacterLife>().Damage(1);
            }
            else
            {
                DetonateOnDeath detonateScript = hit.transform.GetComponent<DetonateOnDeath>();
                if (hit.transform.GetComponent<DetonateOnDeath>())
                    hit.transform.GetComponent<MonsterLife>().Damage(gameObject, 9999f, true);
                else if (hit.transform.GetComponent<MonsterLife>())
                {
                    hit.transform.GetComponent<AIAgent>().KnockBack((hit.transform.position - transform.position).normalized);
                }
            }
        }
    }

    public void StartDetonation() {
        if (!detonateOnTimer)
        {
            detonateOnTimer = true;
            timer = detonateDelay;
        }
    }

    private void Update()
    {
        if (detonateOnTimer && !Pause.Paused) {
            timer -= Time.deltaTime;
            if (timer <= 0) {
                Blast();
            }
        }
    }
}
