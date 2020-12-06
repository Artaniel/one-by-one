using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class SeedInput : MonoBehaviour
{
    public TMPro.TextMeshPro outerSeededText;
    public string sceneToLoad = "LabirintChapter1";
    public GameObject panel;
    public InputField seedInput;

    private void Awake()
    {
        UpdateSeedText();
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
        UpdateSeedText();
        CloseMenu();
    }

    public void CancelButton() {
        seedInput.text = "";
        CloseMenu();
    }

    public void ResetSeed() {
        seedInput.text = "";
        LabirintBuilder.ResetSeed();
        UpdateSeedText();
        CloseMenu();
    }

    private void CloseMenu()
    {
        panel.SetActive(false);
        Pause.SetPause(false, false);
        UnityEngine.Cursor.visible = false;
    }

    private void UpdateSeedText()
    {
        string s = PlayerPrefs.GetString("seed");
        if (s != "")
        {
            seedInput.text = s;
            outerSeededText.text = $"Seed: {s}";
        }
        else
        {
            outerSeededText.text = "Random";
        }
    }
}
