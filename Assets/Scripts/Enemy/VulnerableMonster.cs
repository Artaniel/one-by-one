using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VulnerableMonster : MonsterLife
{
    protected override bool ReceiveFullDamage()
    {
        return true;
    }
}
