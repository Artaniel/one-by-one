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
        monsterLife.OnThisDead.AddListener(DeathCheck);
    }

    private void DeathCheck()
    {
        if (monsterLife.HP <= 0) {
            FireOnTilemap.StartAreaFire(transform.position, 2, 2, firePrefab);
            //FireOnTilemap.StartFire(transform.position, firePrefab);
        }
    }
}
