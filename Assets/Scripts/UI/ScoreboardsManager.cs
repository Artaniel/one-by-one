using System.Collections;
using System.Collections.Generic;
using Game.Events;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreboardsManager : MonoBehaviour
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
    private AlphaManager scoreboardsAlphaManager;
    [SerializeField]
    private List<AlphaManager> inputAlphaManagers = new List<AlphaManager>();
    

    private bool pressed = false;

    private void Start()
    {
        scoreboardsAlphaManager.HideImmediate();
        foreach (var manager in inputAlphaManagers)
            manager.HideImmediate();
        
        EventManager.OnAlphaManagerComplete.AddListener(OnEndFadeout);
    }

    void Update()
    {
        scoreboardsAlphaManager.Update(Time.deltaTime);
        foreach (var manager in inputAlphaManagers)
            manager.Update(Time.deltaTime);

        if (!pressed && Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(ShowInput());
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
    }

    public void HideScoreboards()
    {
        scoreboardsAlphaManager.Hide();
    }

    public void OnEndFadeout(string managerName)
    {
        if (managerName == "Scoreboard_InputField")
            scoreboardsAlphaManager.Show();
        else if (managerName == "Scoreboard_Scoreboards")
            SceneManager.LoadScene(nextScene);
    }

    public void SubmitInput()
    {
        Debug.Log($"User entered {inputField.text}");
        HideInput();
    }
}
