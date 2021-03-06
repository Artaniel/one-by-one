﻿using System.Collections;
using System.Collections.Generic;
using Game.Events;
using Game.Network;
using Game.Network.Scoreboard;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreboardsManagerUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField = null;
    [SerializeField] private Button submitButton = null;
    [SerializeField] private Button skipName = null;
    [SerializeField] private string nextScene = "FinalCredits";
    [SerializeField] private List<AlphaManager> inputAlphaManagers = new List<AlphaManager>();

    [SerializeField] private Transform NormalScoreboard = null;
    [SerializeField] private Transform HardcoreScoreboard = null;
    [SerializeField] private AlphaManager scoreboardsAlphaManager = null;

    [SerializeField] private AlphaManager errorAlphaManager = null;

    [SerializeField] private GameObject errorReturnToMenu = null;
    [SerializeField] private GameObject scoreboardReturnToMenu = null;

    [SerializeField] private AlphaManager loadingAlphaManager = null;
    
    [SerializeField] private string scoreboardServer = "https://gd64.pythonanywhere.com/";
    
    private bool normalBoardRetrieved = false;
    private bool hardcoreBoardRetrieved = false;
    private bool boardsShown = false;
    
    private void Start()
    {
        Ping();
        Cursor.visible = true;
        errorReturnToMenu.SetActive(false);
        scoreboardReturnToMenu.SetActive(false);      
        
        errorAlphaManager.HideImmediate();
        scoreboardsAlphaManager.HideImmediate();
        loadingAlphaManager.HideImmediate();
        
        foreach (var manager in inputAlphaManagers)
            manager.HideImmediate();
        
        EventManager.OnAlphaManagerComplete.AddListener(OnEndFadeout);
        StartCoroutine(ShowInput());
        CreateNextSceneName();
    }

    void Update()
    {
        loadingAlphaManager.Update(Time.deltaTime);
        errorAlphaManager.Update(Time.deltaTime);
        scoreboardsAlphaManager.Update(Time.deltaTime);
        foreach (var manager in inputAlphaManagers)
            manager.Update(Time.deltaTime);

        if (normalBoardRetrieved && hardcoreBoardRetrieved && !boardsShown)
            ShowScoreboards();
    }

    IEnumerator ShowInput()
    {
        inputAlphaManagers[0].Show();
        
        yield return new WaitForSeconds(2f);
        inputAlphaManagers[1].Show();
        
        yield return new WaitForSeconds(2f);
        inputField.interactable = true;
        submitButton.interactable = true;
        skipName.interactable = true;
        
        inputAlphaManagers[2].Show();
    }

    public void HideInput()
    {
        inputField.interactable = false;
        submitButton.interactable = false;
        skipName.interactable = false;
        foreach (var manager in inputAlphaManagers)
            manager.Hide();

        loadingAlphaManager.Show();
    }

    public void HideScoreboards()
    {
        scoreboardsAlphaManager.Hide();
    }

    public void HideErrorScreen()
    {
        errorAlphaManager.Hide();
    }

    public void OnEndFadeout(string managerName)
    {
        if (managerName == "Scoreboard_InputField")
            RetrieveScoreboards();
        else if (managerName == "Scoreboard_Scoreboards" || managerName == "Scoreboard_Error")
        {
            SaveLoading.SaveCurrentScene("Hub");
            SceneLoading.LoadScene(nextScene);
        }
    }

    private void CreateNextSceneName()
    {
        if (SaveLoading.difficulty == 2)
        {
            nextScene = "HardFinalCredits";
        }
    }

    public void SubmitInput()
    {
        Metrics.LoadMetrics();
        float playtime = 0f;
        foreach (var levelTime in Metrics.MetricsContainer.levelTime)
            playtime += levelTime;   
        
        Debug.Log(playtime);

        var plrStats = new ScoreboardSendEntry
        {
            playerName = inputField.text,
            gameTime = playtime
        };
        PostResult(JsonUtility.ToJson(plrStats));

        HideInput();
    }

    public void SkipInput()
    {
        HideInput();
    }

    private void ShowScoreboards()
    {
        boardsShown = true;
        scoreboardsAlphaManager.Show();
        scoreboardReturnToMenu.SetActive(true);
    }

    private void ShowError()
    {
        errorAlphaManager.Show();
        errorReturnToMenu.SetActive(true);
    }
    
    private void RetrieveScoreboards()
    {
        Debug.Log("RetrieveScoreboard() is called");
        StartCoroutine(NetRequester.GetRequest(
            url: scoreboardServer + "/api/scoreboard/normal/",
            onFulfilled: (code, jsonText) =>
            {
                loadingAlphaManager.Hide();
                if (code == 200)
                {
                    var easyList = JsonDeserializer.CreateFromJSON<ScoreboardList>(jsonText);

                    for (int i = 0; i < easyList.entries.Count; i++)
                    {
                        var entry = easyList.entries[i];
                        var item = NormalScoreboard.GetChild(i);
                        var textField = item.GetComponentInChildren<TextMeshProUGUI>();
                        textField.text = $"{entry.place}. {entry.playerName}; {entry.gameTime}";
                    }

                    normalBoardRetrieved = true;
                }
                else ShowError();
            },
            onRejected: (message) =>
            {
                loadingAlphaManager.Hide();
                Debug.Log(message);
                ShowError();
            }
        ));
        
        StartCoroutine(NetRequester.GetRequest(
            url: scoreboardServer + "/api/scoreboard/hardcore/",
            onFulfilled: (code, jsonText) =>
            {
                loadingAlphaManager.Hide();
                if (code == 200)
                {
                    var easyList = JsonDeserializer.CreateFromJSON<ScoreboardList>(jsonText);
                    
                    for (int i = 0; i < easyList.entries.Count; i++)
                    {
                        var entry = easyList.entries[i];
                        var item = HardcoreScoreboard.GetChild(i);
                        var textField = item.GetComponentInChildren<TextMeshProUGUI>();
                        textField.text = $"{entry.place}. {entry.playerName}; {entry.gameTime}";
                    }

                    hardcoreBoardRetrieved = true;
                }
                else ShowError();
            },
            onRejected: (message) =>
            {
                loadingAlphaManager.Hide();
                Debug.Log(message);
                ShowError();
            }
        ));
    }

    private void PostResult(string data)
    {
        string gamemode = PlayerPrefs.GetString("Gamemode");
        string difficulty = "";
        if (gamemode == "1") difficulty = "normal";
        else if (gamemode == "2") difficulty = "hardcore";
        else difficulty = "unknown";
        Debug.Log("PostResult() is called");
        StartCoroutine(NetRequester.PostRequest(
            url: scoreboardServer + $"/api/scoreboard/{difficulty}/", 
            json: data,
            onFulfilled: (code, jsonResponse) =>
            {
                Debug.Log("Player result submitted successfully");
            },
            onRejected: (message) =>
            {
                Debug.Log(message);
                ShowError();
            }
        ));
    }

    private void Ping()
    {
        StartCoroutine(NetRequester.GetRequest(
            url: scoreboardServer + "/api/scoreboard/hardcore/",
            onFulfilled: (code, jsonText) =>
            {
                Debug.Log($"Ping to server returns code: {code}");
            },
            onRejected: (message) =>
            {
                Debug.Log($"Ping failed. {message}");
            }
        ));
    }
}
