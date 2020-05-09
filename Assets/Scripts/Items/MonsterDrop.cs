using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDrop : Container
{
    public float anyDropChance = 1f;
    private MonsterLife monsterLife;

    protected override void Awake()
    {
        monsterLife = GetComponent<MonsterLife>();
        if (Random.Range(0, 1f) <= anyDropChance)
        {
            monsterLife.hpChangedEvent.AddListener(DeathCheck);
            base.Awake();
        }
    }

    private void DeathCheck() {
        if (monsterLife.HP <= 0) { 
            //animation?
            Open();
        }
    }
}
