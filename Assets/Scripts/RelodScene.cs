using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine.Events;

public class RelodScene : MonoBehaviour
{
    [SerializeField]
    protected string NextSceneName = "";
    [SerializeField]
    protected int SceneNumber = 0;

    public bool isPointVictory = false;
    public int pointsToVictory;
    // How much monsters should be spawned after limit is exceeded (not exactly, waves are not cut)
    public int monsterAdditionLimit = 12;
    public static bool isVictory = false;
    public int TotalValue = 0;
    private float maxvalue = 0;

    protected GameObject Canvas;

    public static UnityEvent OnSceneChange = new UnityEvent();

    protected virtual void Awake()
    {
        PlayerPrefs.SetInt("CurrentScene", SceneManager.GetActiveScene().buildIndex);
        CharacterLife.isDeath = false;
        Canvas = GameObject.FindGameObjectWithTag("Canvas");
        var arena = GetComponent<ArenaEnemySpawner>();
        if (arena) maxvalue = arena.EnemyCount();

        Canvas.transform.GetChild(0).gameObject.SetActive(false);
        isVictory = false;
    }

    protected virtual void Start()
    {
        MonsterLife.OnEnemyDead.AddListener(UpdateScoreByOne);
    }

    private void UpdateScoreByOne()
    {
        UpdateScore(1);
    }

    public virtual void UpdateScore(int val = 1)
    {
        TotalValue = TotalValue + val;
        CheckVictoryCondition();
    }

    private bool experimentalReloadRoom = false; 

    protected virtual void Update()
    {
        if (CharacterLife.isDeath) PressR();

        if (Input.GetKeyDown(KeyCode.R) && (Input.GetKey(KeyCode.LeftControl) || CharacterLife.isDeath))
        {
            //Metrics.OnDeath();
            if (experimentalReloadRoom)
                Labirint.instance.ReloadRoom();
            else
                Reload();
        }            
    }

    protected virtual void ProcessVictory()
    {
        isVictory = true;
        Canvas.transform.GetChild(0).gameObject.SetActive(true);
        if (Input.GetKeyDown(KeyCode.F) && !CharacterLife.isDeath)
        {
            Canvas.transform.GetChild(0).gameObject.SetActive(false);
            SceneLoading.LoadScene(NextSceneName);
            Metrics.OnWin();
            MonsterLife.ClearUsedNames();
            OnSceneChange?.Invoke();
        }
    }

    /// <summary>
    /// Updates isVictory field and returns it
    /// </summary>
    /// <returns></returns>
    protected virtual bool CheckVictoryCondition()
    {
        var pointToVictory = isPointVictory ? pointsToVictory : maxvalue;
        isVictory = TotalValue >= pointToVictory;
        return isVictory;
    }

    protected virtual void Reload()
    {
        TotalValue = 0;
        Canvas.transform.GetChild(1).gameObject.SetActive(false);
        SceneLoading.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void PressR()
    {
        print("kek");
        Canvas.transform.GetChild(1).gameObject.SetActive(true);
    }
}