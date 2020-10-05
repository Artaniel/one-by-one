using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShadowPlacer))]
public class ShadowPlacerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ShadowPlacer myTarget = (ShadowPlacer)target;
        if (GUILayout.Button("Place Shadow"))
        {
            myTarget.PlaceShadows();
            EditorUtility.SetDirty(myTarget);
        }
    }
}
