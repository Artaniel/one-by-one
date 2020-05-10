using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialAddKeyToInventory : MonoBehaviour
{
    public ShootingWeapon keySkill;

    private void Awake()
    {
        SkillManager skillManager = GameObject.FindWithTag("Player").GetComponent<SkillManager>();
        skillManager.InventoryWeaponSkill.Add(keySkill);
    }
}
