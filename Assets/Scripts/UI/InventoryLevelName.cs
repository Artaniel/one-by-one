using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryLevelName : MonoBehaviour
{
    TMPro.TextMeshProUGUI textUI;

    void Start()
    {
        textUI = GetComponent<TMPro.TextMeshProUGUI>();
        textUI.text = SaveLoading.currentLocationName.ToUpper();
    }
}
