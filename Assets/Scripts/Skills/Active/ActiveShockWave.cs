﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ActiveShockWave", menuName = "ScriptableObject/ActiveSkill/ActiveShockWave", order = 12)]
public class ActiveShockWave : ActiveSkill
{
    public float radius = 5f;
    public float force = 10f;
    public float delayedMaxTime = 0.3f;
    public float maxDamage = 10f;
    public GameObject visualEffectPrefab = null;    
    private GameObject player;
    private SkillManager skillManager;
    
    public override void InitializeSkill()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        skillManager = player.GetComponent<SkillManager>();
    }

    protected override void ActivateSkill()
    {
        if (visualEffectPrefab)
        {
            var createdObject = PoolManager.GetPool(visualEffectPrefab, player.transform.position, Quaternion.identity);
            PoolManager.ReturnToPool(createdObject, 4f);
        }

        Vector3 distanceVector;
        if (Labirint.instance)
        {
            foreach (GameObject monster in Labirint.currentRoom.GetComponent<MonsterManager>().monsterList)
            {
                distanceVector = player.transform.position - monster.transform.position;
                distanceVector.z = 0;
                float distance = distanceVector.magnitude;
                if (distance <= radius)
                {
                    skillManager.StartCoroutine(DelayedKnockBack(1 - distance / radius, monster));
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="power">Range from 1 (closest) to 0 (far) based on distance to epicenter</param>
    /// <returns></returns>
    private IEnumerator DelayedKnockBack(float power, GameObject monster)
    {
        yield return new WaitForSeconds(delayedMaxTime - power * delayedMaxTime);
        monster.GetComponent<AIAgent>().KnockBack((monster.transform.position - player.transform.position).normalized * force * Mathf.Lerp(0.25f, 1f, power));
        monster.GetComponent<MonsterLife>().Damage(null, Mathf.Lerp(0.25f, 1f, power) * maxDamage);
    }
}
