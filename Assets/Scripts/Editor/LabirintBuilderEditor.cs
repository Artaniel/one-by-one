using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LabirintBuilder))]
public class LabirintBuilderEditor : Editor
{// this script must be in "Editor" folder
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        LabirintBuilder.Table((LabirintBuilder)target);
    }
}
