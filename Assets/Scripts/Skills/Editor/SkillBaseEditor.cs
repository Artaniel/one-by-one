using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SkillBase), true)]
public class SkillBaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Add skill to database", GUILayout.Height(30)))
        {
            SkillAssetLoader.RegisterSkills();
        }

    }
}
