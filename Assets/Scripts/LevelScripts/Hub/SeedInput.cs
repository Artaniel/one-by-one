using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class SeedInput : MonoBehaviour
{
    public string sceneToLoad = "LabirintChapter1";
    public GameObject panel;
    public InputField seedInput;

    private void Awake()
    {
        string s = SaveLoading.seed;
        if (s != "") seedInput.text = s;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            panel.SetActive(true);
            Pause.SetPause(true, false);
            UnityEngine.Cursor.visible = true;
        }
    }

    public void SetSeed()
    {
        if (seedInput.text != "")
        {
            LabirintBuilder.SetupSeed(seedInput.text);
        }
        else
            LabirintBuilder.ResetSeed();
        panel.SetActive(false);
        Pause.SetPause(false, false);
        UnityEngine.Cursor.visible = false;
    }

    public void CancelButton() {
        panel.SetActive(false);
        Pause.SetPause(false, false);
        UnityEngine.Cursor.visible = false;
    }

    public void ResetSeed() {
        seedInput.text = "";
        LabirintBuilder.ResetSeed();
    }
}
