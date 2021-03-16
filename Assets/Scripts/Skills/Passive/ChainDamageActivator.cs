using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Chain Damage Activator", menuName = "ScriptableObject/PassiveSkill/ChainDamageActivator", order = 1)]
public class ChainDamageActivator : PassiveSkill
{
    public override void InitializeSkill()
    {
        CharacterShooting.allowChainDamage = true;
    }
}
