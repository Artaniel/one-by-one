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

    private bool loading = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        panel.SetActive(true);
        Pause.SetPause(true, false);
        UnityEngine.Cursor.visible = true;
    }

    public void LaunchButton()
    {
        LabirintBuilder.SetupSeed(seedInput.text);
        if (!loading)
        {
            loading = true;
            RelodScene.OnSceneChange?.Invoke();
            SceneLoading.LoadScene(sceneToLoad);
        }
    }

    public void CancelButton() {
        panel.SetActive(false);
        Pause.SetPause(false, false);
        UnityEngine.Cursor.visible = false;
    }
}
