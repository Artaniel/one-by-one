using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spawn Skill", menuName = "ScriptableObject/ActiveSkill/SpawnSkill", order = 1)]
public class SkillSpawn : ActiveSkill
{
    public GameObject toSpawn = null;

    public Vector3 offset;
    public bool offsetIsRotationBased = false;
    public bool keepParentRotation = true;
    public bool destroyOnEndOfSkill = false;
    public bool allowMultipleInstances = true;

    public override void InitializeSkill()
    {
        base.InitializeSkill();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected override void ActivateSkill()
    {
        if (!allowMultipleInstances && spawnedInstance) PoolManager.ReturnToPool(spawnedInstance);
        spawnedInstance = 
            PoolManager.GetPool(
                toSpawn, 
                player.position +
                    (offsetIsRotationBased ? player.TransformVector(offset) : offset),
                keepParentRotation ? player.rotation : Quaternion.identity);
    }

    protected override void EndOfSkill()
    {
        base._EndOfSkill();
        if (destroyOnEndOfSkill) PoolManager.ReturnToPool(spawnedInstance);
    }

    private GameObject spawnedInstance;
    private Transform player;
}
