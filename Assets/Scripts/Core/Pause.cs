using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;

public class Pause : MonoBehaviour
{
    public static bool Paused { get; private set; } = false;
    public static bool UnPaused { get { return !Paused; } }
    public static bool AllowPause = true;

    [SerializeField] GameObject pauseCanvas = null;
    [SerializeField] private PostProcessVolume postProcess;
    [SerializeField] private GameObject settings = null;

    private void Awake()
    {
        AllowPause = true;
        if (pauseCanvas != null)
        {
            var pause = Instantiate(pauseCanvas);
            Paused = false;
            myTransform = pause.transform;
            postProcess = myTransform.GetComponentInChildren<PostProcessVolume>();
            ChangeMenuVisibility();
        }
    }

    private void OnApplicationFocus()
    {
        Cursor.visible = Paused;
        CharacterShooting.GetCursor().gameObject.SetActive(!Paused);
    }

    public static void ChangeMenuVisibility()
    {
        for (int i = 0; i < myTransform.childCount; i++)
        {
            myTransform.GetChild(i).gameObject.SetActive(Paused);
        }
    }

    public static void SetPause(bool shouldPause, bool openMenu = true)
    {
        Time.timeScale = shouldPause ? 0 : 1;
        Paused = shouldPause;
        Cursor.visible = shouldPause;
        CharacterShooting.GetCursor().gameObject.SetActive(!shouldPause);

        if (openMenu) ChangeMenuVisibility();
        if (shouldPause)
        {
            AudioManager.PauseMusic();
        }
        else
        {
            AudioManager.ResumeMusic();
        }
    }

    public void ResumeGame()
    {
        SetPause(false);
        AudioManager.ResumeMusic();
    }

    private void Update()
    {
        if (!pauseCanvas) return;
        if ((Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape)) && AllowPause && !InventoryManager.opened)
        {
            SetPause(!Paused);
        }
        if (Paused)
        {
            postProcess.weight = Mathf.Clamp(postProcess.weight + Time.unscaledDeltaTime, 0, 1);
        }
        else
        {
            postProcess.weight = 0;
        }
    }

    public void GoToMenu()
    {
        SetPause(false);
        SceneLoading.LoadScene("MainMenu");
    }

    public void ExitSave()
    {
        Application.Quit();
    }

    public void OpenSettings()
    {
        settings.SetActive(true);
    }

    private static Transform myTransform;
}
