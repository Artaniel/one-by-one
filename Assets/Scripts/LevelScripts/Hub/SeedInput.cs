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
        SetSeed();
    }

    private void OnTriggerEnter2D(UnityEngine.Collider2D collision)
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
        UpdateSeedText(seedInput.text);
        CloseMenu();
    }

    public void CancelButton() {
        seedInput.text = "";
        CloseMenu();
    }

    public void ResetSeed() {
        seedInput.text = "";
        LabirintBuilder.ResetSeed();
        UpdateSeedText(seedInput.text);
        CloseMenu();
    }

    private void CloseMenu()
    {
        panel.SetActive(false);
        Pause.SetPause(false, false);
        UnityEngine.Cursor.visible = false;
    }

    private void UpdateSeedText(string seed)
    {
        if (seed != "")
        {
            seedInput.text = seed;
            outerSeededText.text = $"Seed: {seed}";
        }
        else
        {
            outerSeededText.text = "Random";
        }
    }
}
