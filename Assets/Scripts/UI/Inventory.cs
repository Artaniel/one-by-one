using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] private Transform activeItemsContainer = null;
    [SerializeField] private Transform weaponItemsContainer = null;
    [SerializeField] private Transform passiveSkillsContainer = null;
    [SerializeField] private Transform draggingParent = null;
    [SerializeField] public GameObject cellPrefab = null;
    [SerializeField] private TMPro.TextMeshProUGUI tooltipText = null;
    public Sprite weaponActiveFrame = null;
    public Sprite weaponBaseFrame = null;
    public Sprite weaponEmptyFrame = null;
    public Sprite activeActiveFrame = null;
    public Sprite activeBaseFrame = null;
    public Sprite activeEmptyFrame = null;

    public void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        skills = player.GetComponent<SkillManager>();
        nonEquippedWeaponSkills = new List<SkillBase>();
        equippedWeaponSkills = new List<SkillBase>();
        nonEquippedActiveSkills = new List<SkillBase>();
        equippedActiveSkills = new List<SkillBase>();
        passiveSkills = new List<SkillBase>();
        MakeContainer(activeItemsContainer);
        MakeContainer(weaponItemsContainer);
        AddActiveSkills();
        AddWeaponSkills();
        AddPassiveSkills();
        isStarted = true;
    }

    public void AddSkill(SkillBase skill)
    {
        if(skill is ActiveSkill)
        {
            RebootContainer(activeItemsContainer);
            AddActiveSkills();
        }
        else if(skill is PassiveSkill)
        {
            AddPassiveSkills();
        }
        else if(skill is WeaponSkill) 
        {
            RebootContainer(weaponItemsContainer);
            AddWeaponSkills();
        }
    }

    private void AddActiveSkills()
    {
        if (nonEquippedActiveSkills != null)
            nonEquippedActiveSkills.Clear();
        if (equippedActiveSkills != null)
            equippedActiveSkills.Clear();
        skills.InventoryActiveSkills.ForEach(skill => nonEquippedActiveSkills.Add(skill));
        skills.ActiveSkills.ForEach(skill => equippedActiveSkills.Add(skill.logic));
        Render(equippedActiveSkills, activeItemsContainer, true);
        Render(nonEquippedActiveSkills, activeItemsContainer, false, iFrom: equippedActiveSkills.Count);
    }

    private void AddWeaponSkills()
    {
        if (nonEquippedWeaponSkills.Count > 0)
            nonEquippedWeaponSkills.Clear();
        if (equippedWeaponSkills.Count > 0)
            equippedWeaponSkills.Clear();

        skills.InventoryWeaponSkill.ForEach(skill => nonEquippedWeaponSkills.Add(skill));
        skills.EquippedWeapons.ForEach(weapon => equippedWeaponSkills.Add(weapon.logic));
        Render(equippedWeaponSkills, weaponItemsContainer, true);
        Render(nonEquippedWeaponSkills, weaponItemsContainer, false, iFrom: equippedWeaponSkills.Count);
    }

    private void AddPassiveSkills()
    {
        passiveSkills.Clear();
        foreach (var skill in skills.skills)
        {
            if (skill is PassiveSkill)
                passiveSkills.Add(skill);
        }
        PassiveRender(passiveSkills, passiveSkillsContainer);
    }

    private void RebootContainer(Transform container)
    {
        for (int i = 0; i < container.childCount; i++)
        {
            var cell = container.GetChild(i);
            if (cell.childCount > 0)
            {
                for (int j = 0; j < cell.childCount; j++)
                    cell.GetChild(2).GetComponent<Image>().sprite = cellPrefab.GetComponent<Image>().sprite;
            }
            MakeFrame(cell.gameObject, weaponEmptyFrame);
        }
    }

    private void Render(List<SkillBase> items, Transform container, bool isActive, int iFrom = 0)
    {
        int k = 0;
        for(int i = iFrom; i < container.childCount; i++)
        {
            var empCell = container.GetChild(i);
            if (k < items.Count)// && empCell.GetChild(2).GetComponent<Image>().sprite == cellPrefab.GetComponent<Image>().sprite)
            {
                if (isActive) MakeFrame(empCell.gameObject, weaponActiveFrame);
                else MakeFrame(empCell.gameObject, weaponBaseFrame);
                var skillImage = empCell.GetChild(2).GetComponent<InventoryItemPresenter>();
                skillImage.Init(draggingParent);
                skillImage.Render(items[k], this);
                k++;
            }
        }
    }

    private void MakeContainer(Transform container)
    {
        for (int i = 0; i < container.childCount; i++)
        {
            var empCell = container.GetChild(i);
            MakeFrame(empCell.gameObject, weaponEmptyFrame);
            Instantiate(cellPrefab, empCell);
        }
    }

    private void PassiveRender(List<SkillBase> items, Transform container)
    {
        for (int i = 0; i < items.Count; i++)
        {
            var img = container.GetChild(i).GetComponent<PassiveItemPresenter>();
            img.Render(items[i], this);
        }
    }

    public void OnCellClick(SkillBase currentSkill, Transform cell)
    {
        if (currentSkill is ActiveSkill) 
        {
            // этот массив ищет совпадение между нажимаемой кнопкой и скилами на панели
            var equippedActiveSkill = skills.ActiveSkills.FindAll(skill => skill.logic == currentSkill);
            if (equippedActiveSkill.Count != 0 && equippedActiveSkill[0].cooldown == 0)
            {
                skills.ActiveSkills.RemoveAll(skill => skill.logic == currentSkill);
                var nonActiveList = skills.InventoryActiveSkills;
                nonActiveList.Add(currentSkill as ActiveSkill);
                MakeFrame(cell.parent.gameObject, weaponBaseFrame);
                skills.RefreshUI();
            }
            else if (equippedActiveSkill.Count == 0 && skills.ActiveSkills.Count < SkillManager.maxEquippedActiveCount)
            {
                skills.EquipActiveSkill(currentSkill as ActiveSkill);
                var nonActiveList = skills.InventoryActiveSkills;
                nonActiveList.Remove(currentSkill as ActiveSkill);
                MakeFrame(cell.parent.gameObject, weaponActiveFrame);
            }
        }
        else if (currentSkill is WeaponSkill)
        {
            // этот массив ищет совпадение между нажимаемой кнопкой и скилами на панели
            var equippedWeapon = skills.EquippedWeapons.FindAll(skill => skill.logic == currentSkill);
            if (equippedWeapon.Count != 0) skills.ReloadWeaponIfNeeded(playSound: false); // Не позволим менять разряженное оружие
            if (equippedWeapon.Count != 0 && equippedWeapon[0].reloadTimeLeft == 0)
            {
                List<SkillManager.EquippedWeapon> tmpList = new List<SkillManager.EquippedWeapon>();
                skills.EquippedWeapons.RemoveAll(skill => skill.logic == currentSkill);
                skills.EquippedWeapons.ForEach(skill => tmpList.Add(skill));
                skills.ClearWeapons();
                if (tmpList.Count > 0) tmpList.ForEach(skill => skills.AddSkill(skill.logic));
                else skills.RefreshUI();
                var nonActiveList = skills.InventoryWeaponSkill;
                nonActiveList.Add(currentSkill as WeaponSkill);
                MakeFrame(cell.parent.gameObject, weaponBaseFrame);
            }
            else if (equippedWeapon.Count == 0 && skills.EquippedWeapons.Count < SkillManager.maxEquippedWeaponCount)
            {
                skills.EquipWeapon(currentSkill as WeaponSkill);
                var nonActiveList = skills.InventoryWeaponSkill;
                nonActiveList.Remove(currentSkill as WeaponSkill);
                MakeFrame(cell.parent.gameObject, weaponActiveFrame);
            }
        }
        skills.RefreshUI();
    }

    public static void MakeFrame(GameObject cell, Sprite frame)
    {
        cell.transform.GetChild(1).GetComponent<Image>().sprite = frame;
    }

    #region Tooltip

    struct Tooltip
    {
        string name;
        string stats;
        string description;
    }

    public void UpdateToolTip(SkillBase skill)
    {
        tooltipText.text = skill.GetDescription();
    }

    public void RemoveToolTip()
    {
        tooltipText.text = "";
    }

    #endregion

    private List<SkillBase> nonEquippedActiveSkills = null;
    private List<SkillBase> equippedActiveSkills = null;
    private List<SkillBase> nonEquippedWeaponSkills = null;
    private List<SkillBase> equippedWeaponSkills = null;
    private List<SkillBase> passiveSkills = null;
    private SkillManager skills = null;

    public bool isStarted = false;
}
