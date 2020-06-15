using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillsRecord
{
    private const int activeSkillsMaxCount = 10;
    private const int passiveSkillsMaxCount = 255;
    private const int weaponMaxCount = 10;

    public string[] activeSkills;
    public string[] passiveSkills;
    public string[] weapons;
    int i, j, k;

    public string[] equiptedActiveSkils;
    public string[] equiptedWeaponsSkills;
    public int currentWeaponIndex;

    public SkillsRecord(List<SkillBase> skills, List<SkillManager.EquippedActiveSkill> equiptedActive, List<SkillManager.EquippedWeapon> equiptedWeapons, int weaponIndex)
    {
        activeSkills = new string[activeSkillsMaxCount];
        passiveSkills = new string[passiveSkillsMaxCount];
        weapons = new string[weaponMaxCount];

        // indices for arrays ^^^
        i = j = k = 0;

        foreach (var skill in skills)
        {
            if (skill is PassiveSkill)
            {
                passiveSkills[i] = skill.SkillName();
                i++;
            }
            else if (skill is ActiveSkill)
            {
                activeSkills[j] = skill.SkillName();
                j++;
            }
            else
            {
                weapons[k] = skill.SkillName();
                k++;
            }
        }

        equiptedActiveSkils = new string[equiptedActive.Count];
        for (int i = 0; i < equiptedActive.Count; i++)
            equiptedActiveSkils[i] = equiptedActive[i].skill.SkillName();

        equiptedWeaponsSkills = new string[equiptedWeapons.Count];
        for (int i = 0; i < equiptedWeapons.Count; i++)
            equiptedWeaponsSkills[i] = equiptedWeapons[i].logic.SkillName();

        currentWeaponIndex = weaponIndex;
    }
}
