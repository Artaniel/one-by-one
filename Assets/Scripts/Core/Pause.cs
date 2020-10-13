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

    public static void ChangeMenuVisibility()
    {
        for (int i = 0; i < myTransform.childCount; i++)
        {
            myTransform.GetChild(i).gameObject.SetActive(Paused);
        }
    }

    public static void SetPause(bool shouldPause, bool openMenu = true)
    {
        Paused = shouldPause;
        Cursor.visible = shouldPause;

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
            postProcess.weight = Mathf.Clamp(postProcess.weight + Time.deltaTime, 0, 1);
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

    private static Transform myTransform;
}
