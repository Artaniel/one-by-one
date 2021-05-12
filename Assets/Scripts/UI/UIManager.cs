using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static bool showUI = true;

    private static Renderer[] renderers;
    
    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleUI();
        }
    }

    private static void ToggleUI()
    {
        showUI = !showUI;
        foreach(var rend in renderers)
        {
            rend.enabled = showUI;
        }
        CurrentEnemyUI.GetCanvasInstance().SetActive(showUI);
        CharacterShooting.GetCursor().gameObject.SetActive(showUI);
    }

    public static void DisableUI()
    {
        if (showUI) ToggleUI();
    }

    public static void EnableUI()
    {
        if (!showUI) ToggleUI();
    }
}
