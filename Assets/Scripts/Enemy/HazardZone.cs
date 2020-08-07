using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardZone : MonoBehaviour
{
    public bool harmPlayer = true;
    public bool harmEnemies = false;
    public float enemyDamage = 1;
    public float ignoreTime = 0.5f;

    protected virtual void OnTriggerStay2D(Collider2D coll)
    {
        if (Time.time - ignoreTimeStamp < ignoreTime)
        {
            return;
        }

        if (harmEnemies && coll.CompareTag("EnemyCollider"))
        {
            ignoreTimeStamp = Time.time;
            HarmEnemy(coll.gameObject);
        }
        else if (harmPlayer && coll.CompareTag("Player"))
        {
            ignoreTimeStamp = Time.time;
            HarmPlayer(coll.gameObject);
        }
    }

    protected virtual void HarmPlayer(GameObject player)
    {
        player.GetComponent<CharacterLife>().Damage();
    }

    protected virtual void HarmEnemy(GameObject enemy)
    {
        enemy.GetComponentInParent<MonsterLife>().Damage(null, enemyDamage);
    }

    protected float ignoreTimeStamp = 0;
}
