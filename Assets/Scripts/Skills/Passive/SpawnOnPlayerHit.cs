using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New SpawnOnHitPassive", menuName = "ScriptableObject/PassiveSkill/SpawnOnPlayerHit", order = 1)]
public class SpawnOnPlayerHit : PassiveSkill
{
    public GameObject objToSpawn = null;
    public bool considerRotation = false;

    private CharacterLife characterLife;

    public override void InitializeSkill()
    {
        base.InitializeSkill();
        characterLife = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterLife>();
        characterLife.playerHitEvent.AddListener(SpawnOnHit);
    }

    private void SpawnOnHit()
    {
        Quaternion rotation;
        if (considerRotation) rotation = characterLife.transform.rotation;
        else                  rotation = Quaternion.identity;

        Instantiate(objToSpawn, characterLife.transform.position, rotation);
    }
}
