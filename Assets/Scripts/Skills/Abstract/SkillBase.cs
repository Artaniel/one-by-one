﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class SkillBase : ScriptableObject
{
    [Multiline]
    public string shortDescription;
    [Multiline]
    public string fullDescriprion;
    public Sprite pickupSprite;
    public Sprite miniIcon;
    public int price = 0;

    public virtual string GetDescription() { 
        return LocalizationManager.GetTranlationENtoCurrent(fullDescriprion);
    }

    public string SkillName() => $"{GetType()}:{this.name.Substring(0, this.name.IndexOf("(Clone)") == -1 ? this.name.Length : (this.name.IndexOf("(Clone)")))}";

    public abstract void _InitializeSkill();

    public abstract void UpdateEffect();
}
