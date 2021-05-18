using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static bool showUI = true;

    private static Renderer[] renderers;
    private static MaskableGraphic[] images;
    
    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        images = GetComponentsInChildren<MaskableGraphic>();
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

        foreach (var image in images)
        {
            image.enabled = showUI;
        }
        foreach (var rend in renderers)
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
