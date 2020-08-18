using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FireDamageOnMonsters", menuName = "ScriptableObject/PassiveSkill/FireDamageOnMonsters", order = 12)]
public class FireDamageOnMonsters : PassiveSkill
{
    public override void InitializeSkill()
    {
        FireOnTilemap.damageMobsAllowed = true;
        base.InitializeSkill();
    }
}
