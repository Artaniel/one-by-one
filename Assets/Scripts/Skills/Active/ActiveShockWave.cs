using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ActiveShockWave", menuName = "ScriptableObject/ActiveSkill/ActiveShockWave", order = 12)]
public class ActiveShockWave : ActiveSkill
{
    public float radius = 5f;
    public float force = 10f;
    public GameObject visualEffectPrefab = null;    
    private GameObject player;
    
    public override void InitializeSkill()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public override void ActivateSkill()
    {
        float distance = 0;
        if (Labirint.instance) {
            foreach (GameObject monster in Labirint.currentRoom.GetComponent<MonsterManager>().monsterList) {
                distance = Vector3.Distance(player.transform.position, monster.transform.position);
                if (distance <= radius) {
                    monster.GetComponent<AIAgent>().KnockBack((monster.transform.position - player.transform.position).normalized * force * Mathf.Lerp(0f,1f, (radius-distance)/radius));
                }
            }
            if (visualEffectPrefab)
                Instantiate(visualEffectPrefab, player.transform.position, Quaternion.identity);
        }
    }

}
