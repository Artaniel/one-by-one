using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabirintGamemodeHandler : MonoBehaviour
{
    private Labirint labirint;
    public MRDifficultyMod hardcoreLabirint;

    // Start is called before the first frame update
    void Start()
    {
        labirint = Labirint.instance;
        string difficulty = labirint.difficultySetting;
        if (difficulty == "2")
        {
            labirint.commonMRMods.Add(hardcoreLabirint);
            Debug.Log("Hardcore mod!");
        }
        else if (difficulty != "1")
        {
            Debug.LogWarning("What gamemod? Wow");
        }
    }
}
