﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SkillManager : MonoBehaviour
{
    public static List<BulletModifier> temporaryBulletMods = new List<BulletModifier>();
    public EquippedWeapon equippedWeapon;

    [SerializeField, Header("Important")]
    private bool forceSkillRewrite = false;

    #region Skill Register & Load
    public Dictionary<string, SkillBase> registeredSkills = new Dictionary<string, SkillBase>();

    [SerializeField, Tooltip("Skill database-like prefab")]
    private GameObject prefabSkillLoader = null;

    public AudioClip reloadSound = null;
    public AudioClip switchSound = null;

    private float timeRechargeSpeed = 0.01f;
    private float hitRechargeSpeed = 0.15f;
    /// <summary>
    /// Get all skills in-game from database object
    /// </summary>
    public void FillRegisteredSkills()
    {
        if (prefabSkillLoader == null)
        {
            Debug.LogError("Skill loader prefab not assigned! Can't load skills because of that");
        }
        else
        {
            var skillContainer = prefabSkillLoader.GetComponent<SkillPullFromDatabase>();
            if (skillContainer != null)
            {
                foreach (var skill in skillContainer.LoadSkills().Values)
                {
                    registeredSkills.Add(skill.SkillName(), skill);
                }
            }
            else
            {
                Debug.LogError("Skill loader has no database-pull-script assigned! Can't load skills because of that");
            }   
        }
    }

    public bool SaveSkill(string name, SkillBase skill)
    {
        if (!registeredSkills.ContainsKey(name))
        {
            registeredSkills.Add(name, skill);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void PrintRegisteredSkills()
    {
        print($"Skills registered: {registeredSkills.Count}");
        foreach (var skill in registeredSkills.Keys)
        {
            print(skill + " " + registeredSkills[skill]);
        }
    }

    public SkillBase LoadSkill(string name)
    {
        //print(name);
        return registeredSkills[name];
    }

    public void SaveSkills()
    {
        SkillsRecord skillsSavedInfo;
        if (equippedWeapon != null) {
            skillsSavedInfo = new SkillsRecord(skills, activeSkills, equippedWeapons, equippedWeapon.weaponIndex);
        }
        else {
            skillsSavedInfo = new SkillsRecord(skills, activeSkills, equippedWeapons, 0);
        }
        SaveLoading.SaveSkills(skillsSavedInfo);
    }

    /// <summary>
    /// Loads skills by name. Grab skill information from "registered" skills
    /// </summary>
    private void LoadSkills()
    {
        if (!forceSkillRewrite)
        {
            SkillsRecord record = SaveLoading.LoadSkillsSafe();
            skills = new List<SkillBase>();
            LoadActiveSkills(record);
            LoadWeaponSkills(record);
            LoadPassiveSkills(record);
        }
        else {
            SaveSkills();
        }
    }

    private void LoadActiveSkills(SkillsRecord skillsSavedInfo)
    {
        if (skillsSavedInfo.equiptedActiveSkills != null)
        {
            activeSkills = new List<EquippedActiveSkill>();
            foreach (var skill in skillsSavedInfo.equiptedActiveSkills)
            {
                if (!String.IsNullOrEmpty(skill))
                    AddSkill(Instantiate(registeredSkills[skill] as ActiveSkill));
            }
        }
        foreach (var skill in skillsSavedInfo.nonEquiptedActiveSkills)
        {
            if (!String.IsNullOrEmpty(skill))
            {
                var skilInst = Instantiate(registeredSkills[skill] as ActiveSkill);
                skills.Add(skilInst);
                inventoryActiveSkills.Add(skilInst);
            }
        }
    }

    private void LoadWeaponSkills(SkillsRecord skillsSavedInfo)
    {
        if (skillsSavedInfo.nonEquiptedWeapons != null)
        {
            equippedWeapons = new List<EquippedWeapon>();
            foreach (var skill in skillsSavedInfo.equiptedWeaponsSkills)
            {
                if (!String.IsNullOrEmpty(skill))
                    AddSkill(Instantiate(registeredSkills[skill] as WeaponSkill));
            }
        }
        if (equippedWeapons.Count > 0)
        {
            equippedWeapon = equippedWeapons[skillsSavedInfo.currentWeaponIndex];
            attackManager.LoadNewWeapon(equippedWeapon);
            ApplyWeaponSprites();
        }
        foreach (var skill in skillsSavedInfo.nonEquiptedWeapons)
        {
            if (!String.IsNullOrEmpty(skill))
            {
                var skilInst = Instantiate(registeredSkills[skill] as WeaponSkill);
                skills.Add(skilInst);
                inventoryWeaponSkills.Add(skilInst);
            }
        }
    }

    private void LoadPassiveSkills(SkillsRecord skillsSavedInfo)
    {
        foreach (var skill in skillsSavedInfo.passiveSkills)
        {
            if (!String.IsNullOrEmpty(skill)) skills.Add(Instantiate(registeredSkills[skill] as PassiveSkill));
        }
    }

    #endregion

    [Serializable]
    public class EquippedActiveSkill
    {
        public ActiveSkill skill;
        public float cooldown;
        public float activeTimeLeft;

        public EquippedActiveSkill(ActiveSkill skill)
        {
            this.skill = skill;
            cooldown = 0;
            activeTimeLeft = 0;
        }
    }

    [Serializable]
    public class EquippedWeapon
    {
        public WeaponSkill logic;
        public int ammoLeft;
        public float reloadTimeLeft;
        public float lastTimeEquipped;
        public int weaponIndex;
        public AudioClip attackSound;

        public EquippedWeapon(WeaponSkill weapon, int weaponIndex)
        {
            this.logic = weapon;
            ammoLeft = weapon.ammoMagazine;
            reloadTimeLeft = 0;
            this.weaponIndex = weaponIndex;
            attackSound = weapon.attackSound;
            lastTimeEquipped = Time.time;
        }
    }

    private void Awake()
    {
        RelodScene.OnSceneChange.AddListener(SaveSkills);
        skillsUI = GameObject.FindGameObjectWithTag("Canvas").GetComponent<SkillsUI>();
        characterMovement = GetComponent<CharacterMovement>();
    }

    List<WeaponSkill> inventoryWeaponSkills = new List<WeaponSkill>();
    List<ActiveSkill> inventoryActiveSkills = new List<ActiveSkill>();

    public List<WeaponSkill> InventoryWeaponSkill 
    { 
      get { return inventoryWeaponSkills; } 
      set { inventoryWeaponSkills = value; }
    }
    
    public List<ActiveSkill> InventoryActiveSkills
    {
        get { return inventoryActiveSkills; }
        set { inventoryActiveSkills = value; }
    }

    public List<EquippedWeapon> EquippedWeapons
    {
        get { return equippedWeapons; }
        set { equippedWeapons = value; }
    }

    public List<EquippedActiveSkill> ActiveSkills
    {
        get { return activeSkills; }
        set { activeSkills = value; }
    }

    public void ClearWeapons()
    {
        equippedWeapons.ForEach(weapon => skills.Remove(weapon.logic));
        equippedWeapon = null;
        attackManager.currentWeapon = null;
        equippedWeapons.Clear();
        characterMovement.UpdateWeaponType(WeaponSkill.WeaponType.Empty);
        equippedWeapons = new List<EquippedWeapon>();
    }

    public void AddSkill(SkillBase skill)
    {
        skills.Add(skill);
        skill._InitializeSkill();
        if (skill is ActiveSkill)
        {
            if (activeSkills.Count >= maxEquippedActiveCount)
            {
                inventoryActiveSkills.Add(skill as ActiveSkill);
            }
            else EquipActiveSkill(skill as ActiveSkill);
        }
        else if (skill is WeaponSkill)
        {
            if (equippedWeapons.Count >= maxEquippedWeaponCount)
            {
                inventoryWeaponSkills.Add(skill as WeaponSkill);
            }
            else EquipWeapon(skill as WeaponSkill);
        }
        RefreshUI();
    }

    private void Start()
    {
        temporaryBulletMods = new List<BulletModifier>();
        FillRegisteredSkills();
        //PrintRegisteredSkills();

        attackManager = GetComponent<CharacterShooting>();
        if (forceSkillRewrite)
            InitAfterRewrite();
        else
            LoadSkills();
        InitializeSkills();

        if (attackManager && equippedWeapons.Count != 0)
        {
            attackManager.LoadNewWeapon(equippedWeapon, instant: true);
            characterMovement.UpdateWeaponType(equippedWeapon.logic.weaponType);
        }

        MonsterLife.monsterDamaged.AddListener(ChargeSkillsFromDamage);
    }

    private void InitializeSkills()
    {
        foreach (var s in skills)
        {
            s._InitializeSkill();
        }
        if (!equippedWeapon.logic)
            equippedWeapon = equippedWeapons.Count != 0 ? equippedWeapons[0] : null;

        RefreshUI();
    }

    private void InitAfterRewrite()
    {
        foreach (var s in skills)
        {
            if (forceSkillRewrite)
            {
                if (s is ActiveSkill)
                {
                    if (activeSkills.Count >= maxEquippedActiveCount)
                    {
                        inventoryActiveSkills.Add(s as ActiveSkill);
                    }
                    else
                    {
                        activeSkills.Add(new EquippedActiveSkill(s as ActiveSkill));
                    }
                }
                else if (s is WeaponSkill)
                {
                    if (equippedWeapons.Count >= maxEquippedWeaponCount)
                    {
                        inventoryWeaponSkills.Add(s as WeaponSkill);
                    }
                    else
                    {
                        equippedWeapons.Add(new EquippedWeapon(s as WeaponSkill, equippedWeapons.Count));
                    }
                }
            }
        }
    }

    private List<KeyCode> keys = new List<KeyCode>() {
        KeyCode.Q,
        KeyCode.E,
        KeyCode.F
    };

    private List<KeyCode> weaponKeys = new List<KeyCode>
    {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3
    };

    private void Update()
    {
        if (CharacterLife.isDeath) return;
        HandleSkillActivation();
        ActiveSkillUpdate();
        HandleWeaponSwitch();
        WeaponUpdate();
        PassiveUpdate();
        TemporaryModsLowerTimerAndFilter();
    }

    // Check for a key pressed for active skill
    private void HandleSkillActivation()
    {
        for (int i = 0; i < activeSkills.Count; i++)
        {
            if (Input.GetKeyDown(keys[i]) && activeSkills[i].cooldown <= 0f)
            {
                activeSkills[i].skill._ActivateSkill();
                activeSkills[i].activeTimeLeft = activeSkills[i].skill.activeDuration;
                activeSkills[i].cooldown = activeSkills[i].skill.cooldownDuration;
            }
        }
    }

    // Update effect, cooldown and active time left for active skill
    private void ActiveSkillUpdate()
    {
        float[] skillCooldownsProportion = new float[SkillsUI.skillCount];
        bool[] isActiveSkill = new bool[SkillsUI.skillCount];
        for (int i = 0; i < activeSkills.Count; i++)
        {
            activeSkills[i].cooldown = Mathf.Max(0, activeSkills[i].cooldown - (Time.deltaTime * timeRechargeSpeed));

            if (activeSkills[i].activeTimeLeft > 0)
            {
                activeSkills[i].skill.UpdateEffect();
                activeSkills[i].activeTimeLeft = Mathf.Max(0, activeSkills[i].activeTimeLeft - Time.deltaTime);
                if (activeSkills[i].activeTimeLeft <= 0 || activeSkills[i].skill.interrupt)
                {
                    activeSkills[i].activeTimeLeft = 0;
                    activeSkills[i].skill.EndOfSkill();
                }
            }
            skillCooldownsProportion[i] = activeSkills[i].cooldown / activeSkills[i].skill.cooldownDuration;

            isActiveSkill[i] = activeSkills[i].activeTimeLeft > 0;
        }
        skillsUI.UpdateSkillRecoverVisualCooldown(skillCooldownsProportion, isActiveSkill);
    }

    // Update effect of passive skills
    private void PassiveUpdate()
    {
        foreach (var s in skills)
        {
            if (s is PassiveSkill)
            {
                s.UpdateEffect();
            }
        }
    }

    private void HandleWeaponSwitch()
    {
        int weaponSwitchTo = -1;
        for (int i = 0; i < weaponKeys.Count; i++)
        {
            if (Input.GetKeyDown(weaponKeys[i])) weaponSwitchTo = i;
        }

        if (equippedWeapons.Count <= weaponSwitchTo) return;

        if (weaponSwitchTo != -1 && weaponSwitchTo != equippedWeapon.weaponIndex && equippedWeapons.Count != 0)
        {
            if (switchSound) AudioManager.Play(switchSound);
            
            if (equippedWeapon.ammoLeft < equippedWeapon.logic.ammoMagazine)
            {
                ReloadWeaponIfNeeded(playSound: false);
            }

            equippedWeapon = equippedWeapons[weaponSwitchTo];
            characterMovement.UpdateWeaponType(equippedWeapon.logic.weaponType);
            foreach (var weapon in equippedWeapons)
                attackManager.LoadNewWeapon(equippedWeapon);
            ApplyWeaponSprites();
            
        }
    }

    // Update reload time of all weapons & call update
    private void WeaponUpdate()
    {
        float[] weaponCooldownsProportion = new float[SkillsUI.weaponsCount];
        int j = 0;
        foreach (var weapon in equippedWeapons)
        {
            if (weapon.reloadTimeLeft != 0)
            {
                weapon.reloadTimeLeft = Mathf.Max(0, weapon.reloadTimeLeft - Time.deltaTime);
                weapon.ammoLeft = Mathf.Max(weapon.ammoLeft, (int)Mathf.Floor(Mathf.Lerp(weapon.logic.ammoMagazine, 0, (weapon.reloadTimeLeft - 0.01f) / weapon.logic.reloadTime)));
                weaponCooldownsProportion[j] = weapon.reloadTimeLeft / weapon.logic.reloadTime;
            }
            else
            {
                weaponCooldownsProportion[j] =  1 - (float)weapon.ammoLeft / weapon.logic.ammoMagazine;
            }

            weapon.logic.UpdateEffect();
            j++;
        }
        if (equippedWeapons.Count != 0)
        {
            skillsUI.UpdateWeaponReloadVisualCooldown(weaponCooldownsProportion, equippedWeapon.weaponIndex);
            equippedWeapon.logic?.UpdateEquippedEffect();
        }
    }

    // Update temporary weapon mods
    private static void TemporaryModsLowerTimerAndFilter()
    {
        for (int i = 0; i < temporaryBulletMods.Count; i++)
        {
            temporaryBulletMods[i].modifierTime -= Time.deltaTime;
            if (temporaryBulletMods[i].modifierTime <= 0)
            {
                temporaryBulletMods.RemoveAt(i);
                i--;
            }
        }
    }

    public void ReloadWeaponIfNeeded(bool playSound = true)
    {
        if (equippedWeapon.reloadTimeLeft == 0 && equippedWeapon.ammoLeft < equippedWeapon.logic.ammoMagazine)
        {
            if (playSound && equippedWeapon.logic.reloadSound) AudioManager.Play(equippedWeapon.logic.reloadSound);
            equippedWeapon.reloadTimeLeft = equippedWeapon.logic.reloadTime *
                Mathf.Lerp(1, 0.4f, (float)equippedWeapon.ammoLeft / equippedWeapon.logic.ammoMagazine); // more bullets = faster reload
        }
    }

    public void EquipWeapon(WeaponSkill skill)
    {
        equippedWeapons.Add(new EquippedWeapon(skill, equippedWeapons.Count));
        if (equippedWeapons.Count == 1) // There was no other weapons before we added this
        {
            equippedWeapon = equippedWeapons[0];
            attackManager.LoadNewWeapon(equippedWeapon, instant: true);
            characterMovement.UpdateWeaponType(equippedWeapon.logic.weaponType);
        }
        skill.InitializeSkill();
    }

    public void EquipActiveSkill(ActiveSkill skill)
    {
        activeSkills.Add(new EquippedActiveSkill(skill as ActiveSkill));
        skill._InitializeSkill();
    }

    #region UI block
    public void RefreshUI()
    {
        ApplyWeaponSprites();
        ApplySkillSprites();
    }

    public void ApplyWeaponSprites()
    {
        var weaponIcons = new Sprite[SkillsUI.weaponsCount];
        var weaponMiniIcons = new Sprite[SkillsUI.weaponsCount];
        for (int i = 0; i < equippedWeapons.Count; i++)
        {
            weaponIcons[i] = equippedWeapons[i].logic.pickupSprite;
            weaponMiniIcons[i] = equippedWeapons[i].logic.miniIcon;
        }
        skillsUI.SetWeaponSprites(weaponIcons, weaponMiniIcons, equippedWeapons.Count != 0 ? equippedWeapon.weaponIndex : 0);
    }

    public void ApplySkillSprites()
    {
        var skillIcons = new Sprite[SkillsUI.skillCount];
        for (int i = 0; i < activeSkills.Count && i < SkillsUI.skillCount; i++)
        {
            if (activeSkills[i] != null)
            {
                skillIcons[i] = activeSkills[i].skill.pickupSprite;
            }
        }
        skillsUI.SetSkillSprites(skillIcons);
    }

    public void ChargeSkillsFromDamage(float damage, GameObject _)
    {
        foreach (var skill in activeSkills)
        {
            skill.cooldown = Mathf.Max(0, skill.cooldown - (damage * hitRechargeSpeed));
        }
    }
    #endregion

    public const int maxEquippedActiveCount = 3;
    public const int maxEquippedWeaponCount = 3;

    public List<SkillBase> skills = new List<SkillBase>();

    public List<EquippedActiveSkill> activeSkills = new List<EquippedActiveSkill>();

    public List<EquippedWeapon> equippedWeapons = new List<EquippedWeapon>();
    private CharacterShooting attackManager;
    private SkillsUI skillsUI;
    private CharacterMovement characterMovement;
}
