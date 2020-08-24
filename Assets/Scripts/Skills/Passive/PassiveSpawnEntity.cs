using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spawn Entity", menuName = "ScriptableObject/PassiveSkill/SpawnEntity", order = 1)]
public class PassiveSpawnEntity : PassiveSkill
{
    public GameObject entityToSpawn = null;

    public override void InitializeSkill()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        SpawnEntity();

        Room.OnAnyRoomEnter.AddListener(SpawnEntity);
        Room.OnAnyRoomLeave.AddListener(DespawnEntity);
    }

    protected virtual void SpawnEntity()
    {
        entitySpawned = PoolManager.GetPool(entityToSpawn, player.position, Quaternion.identity);
    }

    protected virtual void DespawnEntity()
    {
        PoolManager.ReturnToPool(entitySpawned);
    }

    protected Transform player;
    protected GameObject entitySpawned;
}
