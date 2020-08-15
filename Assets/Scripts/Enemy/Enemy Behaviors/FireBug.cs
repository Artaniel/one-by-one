using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBug : MonoBehaviour
{
    private MonsterLife monsterLife;
    public GameObject firePrefab;

    private void Awake()
    {
        monsterLife = GetComponent<MonsterLife>();
        MonsterLife.OnEnemyDead.AddListener(DeathCheck);
    }

    private void DeathCheck()
    {
        if (monsterLife.HP <= 0) {
            FireOnTilemap.StartFire(transform.position, firePrefab);
        }
    }
}
