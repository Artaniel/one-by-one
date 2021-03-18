using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserRayBullet : BulletLife
{
    [HideInInspector] public Transform startPoint;
    private LineRenderer lineRenderer;
    private Camera cameraMain;

    protected override void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        cameraMain = Camera.main;
        ignoreTime = 0.1f;
    }

    protected override void Move()
    {
        UpdateRay();        
    }

    private void UpdateRay() {
        if (CharacterLife.isDeath || Pause.Paused)
        {
            DestroyBullet();
        }
        else
        {
            LayerMask mask = LayerMask.GetMask("Solid") + LayerMask.GetMask("Default") + LayerMask.GetMask("Flying");
            RaycastHit2D[] hits = Physics2D.RaycastAll(startPoint.position, 
                (cameraMain.ScreenToWorldPoint(Input.mousePosition) - startPoint.position).normalized * 100f, 1000f, mask);
            RaycastHit2D closestHit = hits[0];
            float minDist = Mathf.Infinity;
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.CompareTag("Environment") || (!piercing && hit.collider.CompareTag("EnemyCollider")))
                {
                    if (Vector3.Distance(startPoint.position, hit.point) < minDist)
                    {
                        minDist = Vector3.Distance(startPoint.position, hit.point);
                        closestHit = hit;
                    }
                }
            }
            foreach (RaycastHit2D hit in hits)
            {
                if (Vector3.Distance(startPoint.position, hit.point) <= minDist)
                    if (hit.collider.CompareTag("EnemyCollider"))
                    {
                        ActivateHitEnemyMods(hit.collider);

                        MonsterLife monsterComp = hit.collider.GetComponentInParent<MonsterLife>();
                        if (monsterComp)
                        {
                            DamageMonster(monsterComp);
                        }
                        else
                        {
                            Debug.LogError("ОШИБКА: УСТАНОВИТЕ МОНСТРУ " + hit.collider.gameObject.name + " КОМПОНЕНТ MonsterLife");
                        }
                    }
            }
            lineRenderer.SetPosition(0, startPoint.position);
            lineRenderer.SetPosition(1, closestHit.point);
            transform.position = startPoint.position;
            transform.rotation = startPoint.rotation;
        }
    }

    protected override void CustomInitializeBullet() {
        transform.localScale = startSize;        
        ActivateSpawnMods();        
        bullets.Add(gameObject);
    }

    public override void DestroyBullet() {
        if (destroyed) return;
        destroyed = true;
        ActivateDestroyMods();
        dynamicLightInOut?.FadeOut();
        DeactivateMods();
        bullets.Remove(gameObject);
        PoolManager.ReturnToPool(gameObject, 0);
        lineRenderer.SetPosition(0, startPoint.position);
        lineRenderer.SetPosition(1, startPoint.position);
    }

    protected override void OnTriggerEnter2D(Collider2D coll) { }
}
