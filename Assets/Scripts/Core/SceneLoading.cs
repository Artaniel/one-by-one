using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoading : MonoBehaviour
{
    static public SceneLoading instance;
    [SerializeField] private Canvas loadingCanvas;    
    static private int nextSceneBuildIndex = 0;
    static private string nextSceneName = "";
    static private bool readFromString = true;
    [SerializeField] private float minWaitTime = 5f;

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

    IEnumerator SceneTransition()
    {
        instance.loadingCanvas.enabled = true;
        AsyncOperation asyncOperation;
        if (readFromString)
            asyncOperation = SceneManager.LoadSceneAsync(nextSceneName);
        else
            asyncOperation = SceneManager.LoadSceneAsync(nextSceneBuildIndex);
        asyncOperation.allowSceneActivation = false;

        yield return new WaitForSeconds(minWaitTime);
        yield return asyncOperation.isDone;

        instance.loadingCanvas.enabled = false;
        asyncOperation.allowSceneActivation = true;
    }
}
