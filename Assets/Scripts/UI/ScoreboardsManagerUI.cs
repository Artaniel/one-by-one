using System.Collections;
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
    [SerializeField]
    private TMP_InputField inputField;
    [SerializeField]
    private Button submitButton;
    [SerializeField]
    private Button skipName;
    [SerializeField]
    private string nextScene = "MainMenu";
    [SerializeField]
    private List<AlphaManager> inputAlphaManagers = new List<AlphaManager>();

    [SerializeField] 
    private Transform NormalScoreboard = null;
    [SerializeField] 
    private Transform HardcoreScoreboard = null;
    [SerializeField]
    private GameObject ScoreboardItemPrefab = null;
    [SerializeField] 
    private AlphaManager scoreboardsAlphaManager;

    [SerializeField] 
    private AlphaManager errorAlphaManager;

    [SerializeField] 
    private GameObject errorReturnToMenu;
    [SerializeField]
    private GameObject scoreboardReturnToMenu;

    [SerializeField]
    private AlphaManager loadingAlphaManager;
    
    private bool pressed = false;
    private string scoreboardServer = "http://laptop.lan:10000";

    private bool nameInputSkipped = false;
    
    private bool normalBoardRetrieved = false;
    private bool hardcoreBoardRetrieved = false;
    private bool boardsShown = false;
    

    private void Start()
    {
        errorReturnToMenu.SetActive(false);
        scoreboardReturnToMenu.SetActive(false);      
        
        errorAlphaManager.HideImmediate();
        scoreboardsAlphaManager.HideImmediate();
        loadingAlphaManager.HideImmediate();
        
        foreach (var manager in inputAlphaManagers)
            manager.HideImmediate();
        
        EventManager.OnAlphaManagerComplete.AddListener(OnEndFadeout);
        StartCoroutine(ShowInput());
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
            SceneManager.LoadScene(nextScene);
    }

    public void SubmitInput()
    {
        var plrStats = new ScoreboardSendEntry
        {
            playerName = inputField.text,
            gameTime = 1000f
        };
        PostResult(JsonUtility.ToJson(plrStats));

        HideInput();
    }

    public void SkipInput()
    {
        nameInputSkipped = true;
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
        Debug.Log("PostResult() is called");
        StartCoroutine(NetRequester.PostRequest(
            url: scoreboardServer + "/api/scoreboard/normal/", 
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
}
