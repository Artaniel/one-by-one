using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnOnDeath : MonoBehaviour
{
    public GameObject toSpawn = null;
    public bool keepParentRotation = false;
    public GameObject infusedVFX = null;

    private void Start()
    {
        if (infusedVFX != null)
        {
            Instantiate(infusedVFX, transform);
        }

        var monster = GetComponent<MonsterLife>();
        monster.OnThisDead.AddListener(OnMonsterDeath);
    }

    private void OnApplicationQuit()
    {
        spawnBlock = true;
    }

    private void OnMonsterDeath()
    {
        if (spawnBlock) return;
        var obj = Instantiate(toSpawn, transform.position, keepParentRotation ? transform.rotation : Quaternion.identity);
    }

    public bool spawnBlock = false;
}
