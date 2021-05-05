using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TentacleAreaGrabber : MonoBehaviour
{
    public GameObject tentacleToSpawn;
    public float radius = 10f;
    public float pullPower = 40f;

    List<SpriteRenderer> tentacles = new List<SpriteRenderer>();
    List<MonsterLife> enemies = new List<MonsterLife>();
    List<AIAgent> enemyBodies = new List<AIAgent>();

    private void OnEnable()
    {
        tentacles.Clear();
        enemies.Clear();
        enemyBodies.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (child.gameObject.activeSelf)
            {
                child.transform.SetParent(null);
                PoolManager.ReturnToPool(child.gameObject);
            }
        }

        var enemiesNearby = (from coll in Physics2D.OverlapCircleAll(transform.position, radius)
                       where coll.CompareTag("EnemyCollider")
                       select coll).ToArray();
        foreach (var enemy in enemiesNearby)
        {
            MonsterLife enemyLife = enemy.GetComponentInParent<MonsterLife>(); 
            
            if (enemyLife)
            {
                enemies.Add(enemyLife);
                var tentacle = PoolManager.GetPool(tentacleToSpawn, transform);
                tentacles.Add(tentacle.GetComponent<SpriteRenderer>());
                enemyBodies.Add(enemy.GetComponentInParent<AIAgent>());
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < tentacles.Count; i++)
        {
            if (tentacles[i].gameObject.activeSelf && enemies[i].HP > 0
                && Vector3.Distance(enemies[i].transform.position, transform.position) < radius + 3f)
            {
                SetTentacleTransform(tentacles[i], transform.position, enemies[i].transform.position);
                if (Vector3.Distance(enemies[i].transform.position, transform.position) > 1.5f)
                    enemyBodies[i].KnockBack((transform.position - enemies[i].transform.position).normalized * pullPower * Time.deltaTime);
            }
            else if (tentacles[i].gameObject.activeSelf)
            {
                PoolManager.ReturnToPool(tentacles[i].gameObject);
            }

        }
    }

    private void SetTentacleTransform(SpriteRenderer tentacle, Vector3 from, Vector3 to)
    {
        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, to - from);
        Vector3 position = (from + to) / 2f;
        tentacle.transform.SetPositionAndRotation(position, rotation);
        tentacle.transform.Rotate(0, 0, 90);

        float scaleX = (from - to).magnitude;
        Vector2 tentacleSize = tentacle.size;
        tentacle.size = new Vector2(scaleX, tentacleSize.y);
    }

    private void OnDisable()
    {

    }
}
