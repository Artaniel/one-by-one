using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

public class Metrics : MonoBehaviour
{
    [SerializeField]
    private static MetricsRecords metrics = null;

    public static MetricsRecords MetricsContainer
    {
        get => metrics;
        private set => metrics = value;
    }

    private static string fileName = "metrics03-2.bin";
    private static int sceneIndex;
    private static bool levelIsRuning = true;

    private void Awake()
    {
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (metrics == null) {
            LoadMetrics();
        }
        try
        {
            metrics.levelSceneName[sceneIndex] = SceneManager.GetActiveScene().name;
            metrics.deathRooms = new Dictionary<string, int>();
        }
        catch (System.Exception)
        {
            OnNewGame();
            metrics.levelSceneName[sceneIndex] = SceneManager.GetActiveScene().name;
            metrics.deathRooms = new Dictionary<string, int>();
        }
    }

    private void Update()
    {
        if (!Pause.Paused && levelIsRuning) {
            metrics.levelTime[sceneIndex] += Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.BackQuote)) {
            OutputMetrics();
        }
    }

    private void OnDestroy()
    {
        SaveMetrics(metrics);// to update time on exit to main menu or closing the game
    }

    public static void OnNewGame() {
        metrics = new MetricsRecords();  //to empty arrays
        SaveMetrics(metrics); // overwrite the save
    }

    public static void OnContinueGame() {
        LoadMetrics();
    }

    public static void OnWin() { 
        metrics.levelComlpeted[sceneIndex] = true;
        SaveMetrics(metrics);
    }

    public static void OnDeath() {
        metrics.deathCount[sceneIndex]++;
        SaveMetrics(metrics);
        if (Labirint.instance != null) {
            string roomName = Labirint.GetCurrentRoom().name;
            if (metrics.deathRooms.ContainsKey(roomName))
            {
                metrics.deathRooms[roomName]++;
            }
            else {
                metrics.deathRooms.Add(roomName,1);
            }
        }
    }

    static private void SaveMetrics(MetricsRecords data)
    {
        BinaryFormatter binaryformatter = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + fileName);
        binaryformatter.Serialize(file, data);

        file.Close();
    }

    public static void LoadMetrics() {
        if (File.Exists(Application.persistentDataPath + fileName))
        {
            BinaryFormatter binaryformatter = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + fileName, FileMode.Open);
            try
            {
                if (file.Length > 0)
                {
                    metrics = (MetricsRecords)binaryformatter.Deserialize(file);
                    file.Close();
                }
                else
                {
                    Debug.Log("Empty metrics. Force rewrite.");
                    metrics = new MetricsRecords();
                    file.Close();
                    SaveMetrics(metrics);
                }
            }
            catch (UnityException E) 
            {
                Debug.Log("Unexpected error on metrics load.");
                Debug.Log(E);
                file.Close();
                metrics = new MetricsRecords();
                SaveMetrics(metrics);
            }
        }
        else
        {
            metrics = new MetricsRecords();
            SaveMetrics(metrics); // saving empty file
        }
    }

    private static void OutputMetrics() {
        Debug.Log("metrics output");
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++) {
            if (metrics.levelComlpeted[i]) { // for finished levels
                Debug.Log(i.ToString() + " " + metrics.levelSceneName[i] + " death:" + metrics.deathCount[i] + " time:" + metrics.levelTime[i]);
            }
        }
        if (!metrics.levelComlpeted[sceneIndex]) { //for current scene
            Debug.Log(sceneIndex.ToString() + " " + metrics.levelSceneName[sceneIndex] + " death:" + metrics.deathCount[sceneIndex] + " time:" + metrics.levelTime[sceneIndex] + " unfinished");
        }
    }
}
