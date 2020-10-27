using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetonateOnDeath : MonoBehaviour
{
    [SerializeField] private float playerBlastRadius = 3f;
    [SerializeField] private float bugBlastRadius = 4.5f;
    [SerializeField] private float detonateDelay = 0.5f;
    [SerializeField] private GameObject detonationVFX = null;
    [SerializeField] private float detonateDamageDuration = 0.25f;
    private bool detonateOnTimer = false;
    private float timer = 0f;
    private float detonateTimer = 0f;
    private bool attached = true;
    private GameObject detonationVFXInstance;

    private void Awake()
    {
        GetComponent<MonsterLife>().OnThisDead.AddListener(StartDetonation);
    }

    private void OnEnable()
    {
        attached = true;
    }

    private void Blast()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, bugBlastRadius, Vector3.forward, 0f);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.tag == "Player")
            {
                if (Vector3.Distance(transform.position, hit.transform.position) <= playerBlastRadius)
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
            detonateTimer = detonateDamageDuration;
            timer = detonateDelay;
            detonationVFXInstance = PoolManager.GetPool(detonationVFX, transform);
            GetComponentInChildren<Animator>().Play("Detonate");
        }
    }

    private void Update()
    {
        if (detonateOnTimer && !Pause.Paused) {
            timer -= Time.deltaTime;
            if (timer <= 0 && detonateTimer >= 0) {
                if (attached)
                {
                    attached = false;
                    detonationVFXInstance.transform.SetParent(null);
                }
                detonateTimer -= Time.deltaTime;
                Blast();
            }
        }
    }
}
