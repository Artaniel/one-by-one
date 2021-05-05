using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillsRecord
{
    private const int activeSkillsMaxCount = 255;
    private const int passiveSkillsMaxCount = 255;
    private const int weaponMaxCount = 255;

    public string[] nonEquiptedActiveSkills;
    public string[] passiveSkills;
    public string[] nonEquiptedWeapons;
    int i, j, k;

    public string[] equiptedActiveSkills;
    public string[] equiptedWeaponsSkills;
    public int currentWeaponIndex;

    public SkillsRecord(List<SkillBase> skills, List<SkillManager.EquippedActiveSkill> equiptedActive, List<SkillManager.EquippedWeapon> equiptedWeapons, int weaponIndex)
    {
        nonEquiptedActiveSkills = new string[activeSkillsMaxCount];
        passiveSkills = new string[passiveSkillsMaxCount];
        nonEquiptedWeapons = new string[weaponMaxCount];

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
                nonEquiptedActiveSkills[j] = skill.SkillName();
                j++;
            }
            else
            {
                nonEquiptedWeapons[k] = skill.SkillName();
                k++;
            }
        }

        equiptedActiveSkills = new string[equiptedActive.Count];
        for (int i = 0; i < equiptedActive.Count; i++)
        {
            equiptedActiveSkills[i] = equiptedActive[i].logic.SkillName();
            for (int l = 0; l < j; l++) {
                if (nonEquiptedActiveSkills[l] == equiptedActiveSkills[i]) {
                    nonEquiptedActiveSkills[l] = nonEquiptedActiveSkills[j-1];    // remove equipted skill from array for non-equipted
                    nonEquiptedActiveSkills[j-1] = "";
                    j--;
                    l = j; // brake loop
                }
            }
        }

        equiptedWeaponsSkills = new string[equiptedWeapons.Count];
        for (int i = 0; i < equiptedWeapons.Count; i++)
        {
            equiptedWeaponsSkills[i] = equiptedWeapons[i].logic.SkillName();
            for (int l = 0; l < k; l++) { 
                if (nonEquiptedWeapons[l] == equiptedWeaponsSkills[i]) {
                    nonEquiptedWeapons[l] = nonEquiptedWeapons[k-1];
                    nonEquiptedWeapons[k-1] = "";
                    k--;
                    l = k;
                }
            }
        }
        currentWeaponIndex = weaponIndex;
    }
}
