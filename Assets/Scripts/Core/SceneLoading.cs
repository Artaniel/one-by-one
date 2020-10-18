using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoading : MonoBehaviour
{
    static public SceneLoading instance;
    [SerializeField] private Canvas loadingCanvas = null;    
    static private int nextSceneBuildIndex = 0;
    static private string nextSceneName = "";
    static private bool readFromString = true;
    static private bool ASAP = false;
    [SerializeField] private Image panelImage = null;
    [SerializeField] private Text text = null;

    private static string[] episodes = { "LabirintChapter1", "LabirintChapter2" };

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            loadingCanvas.enabled = false;
        }
        else
            Destroy(gameObject);
    }

    static public void LoadScene(int sceneBuildIndex)
    {
        if (instance)
        {
            nextSceneBuildIndex = sceneBuildIndex; // parametrs for coroutine
            nextSceneName = "";
            readFromString = false;
            instance.StartCoroutine("SceneTransition");
        }
        else
        { // if we started not from main menu, no loading screen exception
            SceneManager.LoadScene(sceneBuildIndex);
        }
    }

    static public void LoadScene(string sceneName)
    {
        if (instance)
        {
            nextSceneName = sceneName;
            nextSceneBuildIndex = -1;
            readFromString = true;
            instance.StartCoroutine("SceneTransition");
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    static public void LoadScene(string sceneName, bool loadASAP) {
        ASAP = loadASAP;
        LoadScene(sceneName);
    }

    IEnumerator SceneTransition()
    {
        instance.loadingCanvas.enabled = true;
        AsyncOperation asyncOperation;
        if (readFromString)
            asyncOperation = SceneManager.LoadSceneAsync(nextSceneName);
        else
            asyncOperation = SceneManager.LoadSceneAsync(nextSceneBuildIndex);
        asyncOperation.allowSceneActivation = false;

        const float minWaitTime = 1.5f;
        const float fadeTime = 0.5f;
        float fadePhase;
        float startTime = Time.time;
        while (Time.time <= startTime + fadeTime) {
            fadePhase = (Time.time - startTime) / fadeTime;
            text.color = new Color(text.color.r, text.color.g, text.color.b, fadePhase);
            panelImage.color = new Color(panelImage.color.r, panelImage.color.g, panelImage.color.b, fadePhase);
            yield return null;
        }

        if (!ASAP)
            yield return new WaitForSeconds(minWaitTime - 2*fadeTime);
        yield return asyncOperation.isDone;
        
        asyncOperation.allowSceneActivation = true;

        startTime = Time.time;
        while (Time.time <= startTime + fadeTime)
        {
            fadePhase = (Time.time - startTime) / fadeTime;
            text.color = new Color(text.color.r, text.color.g, text.color.b, 1 - fadePhase);
            panelImage.color = new Color(panelImage.color.r, panelImage.color.g, panelImage.color.b, 1 - fadePhase);
            yield return null;
        }

        instance.loadingCanvas.enabled = false;
        ASAP = false; //clear varible for next use
    }

    static public void CompleteEpisode(int episodeID)
    {
        HubEpisodeAvailabilityManager.EpisodeComplited(episodeID);
        NextLevel(episodes[episodeID + 1]);
        //LoadScene("Hub");
    }

    static public void NextLevel(string nextSceneName) { 
        LoadScene(nextSceneName);
    }
}
