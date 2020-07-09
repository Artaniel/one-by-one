using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTipDynamic : MonoBehaviour
{
    public Vector3[] weaponTipPositions;

    public void ChoosePosition(WeaponSkill.WeaponType weaponType)
    {
        transform.localPosition = weaponTipPositions[(int)weaponType];
    }
}
