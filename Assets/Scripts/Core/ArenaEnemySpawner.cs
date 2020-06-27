using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class ArenaEnemySpawner : MonoBehaviour
{
    public Vector2 RoomBounds = new Vector2(15, 10);

    [SerializeField]
    private float timeToEachSpawn = 5;
    [SerializeField]
    private float timeToNextSpawn = 0;

    [SerializeField]
    protected GameObject[] enemyWaves = null;



    public ZoneScript SpawnZone = null;

    [SerializeField]
    protected bool AllowEarlySpawns = true;

    [SerializeField]
    private bool isInfSpawn = false;

    public static int boysCount = 0;

    public bool labirintMode = false;

    void Awake()
    {
        if (GameObject.FindGameObjectWithTag("Room") != null)  // for room in labirint variation
            labirintMode = true;
        roomLighting = GetComponent<RoomLighting>();
        scenesController = GetComponent<RelodScene>();
        isPointVictory = scenesController.isPointVictory;

        if (!labirintMode)
        {
            GameObject SpawnSquare = GameObject.FindGameObjectWithTag("SpawnZone");
            if (SpawnSquare)
            {
                SpawnZone = SpawnSquare.GetComponent<ZoneScript>();
            }
        }
    }

    void Start()
    {
        InitializeFields();
    }

    private void InitializeFields()
    {
        // Listens for "Enemy dead" event to lower the number of enemies on screen
        MonsterLife.OnEnemyDead.AddListener(LowerBoysCount);

        boysList = new List<GameObject>();
        boysCount = 0;
        enemiesCount = baseEnemyCount();
    }

    //public static void ChangeTheBoy(GameObject oldBoy)
    //{
    //    if (scenesController)
    //    {
    //        scenesController.UpdateScore(1);
    //    }
    //    roomLighting.AddToLight(1);

    //    boysList.Remove(oldBoy);
    //    if (boysList.Count != 0)
    //    {
    //        var nextBoy = boysList[Random.Range(0, boysList.Count)];
    //        CurrentEnemyUI.SetCurrentEnemy(nextBoy);
    //        nextBoy.GetComponent<MonsterLife>().MakeBoy();
    //        currentBoy = nextBoy;
    //    }
    //    else
    //    {
    //        anyBoy = false;
    //    }
    //}

    private Vector2 RandomBorderSpawnPos()
    {
        var spawnPosition = new Vector2();
        Debug.LogError("Use MonsterManager for handling monsters");
        return spawnPosition;
    }

    protected void SetMonsterPosition(GameObject enemy)
    {
        enemy.transform.position = RandomBorderSpawnPos();
    }

    private void SpawnMonsters(int waveNum)
    {
        Debug.LogError("Use MonsterManager for handling monsters");
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (Pause.Paused) return;

        EnemySpawnUpdate();
        if (Labirint.instance == null)
            if (RelodScene.isVictory)
            {
                KillThemAll();
            }
    }

    public void KillThemAll()
    {
        Debug.LogError("Use MonsterManager for handling monsters");
    }

    protected void EnemySpawnUpdate()
    {
        Debug.LogError("Use MonsterManager for handling monsters");
    }

    public int EnemyCount()
    {
        return isPointVictory ? scenesController.pointsToVictory : baseEnemyCount();
    }

    public int baseEnemyCount()
    {
        enemiesCount = 0;
        foreach (var e in enemyWaves)
        {
            enemiesCount += e.transform.childCount;
        }
        return enemiesCount;
    }

    /// <summary>
    /// Spawn the monster with random name
    /// </summary>
    /// <param name="monster"></param>
    /// <returns></returns>
    public GameObject SpawnMonster(GameObject monster)
    {
        var enemy = Instantiate(monster, transform.position, Quaternion.identity);
        boysList.Add(enemy);

        if (!SpawnZone)
        {
            SetMonsterPosition(enemy);
        }
        else
        {
            enemy.transform.position = SpawnZone.RandomZonePosition();
        }

        sequenceIndex++;
        return enemy;
    }

    public GameObject SpawnMonster(GameObject monster, string name)
    {
        var createdMonster = SpawnMonster(monster);
        createdMonster.GetComponentInChildren<TMPro.TextMeshPro>().text = name;
        return createdMonster;
    }

    private void LowerBoysCount()
    {
        boysCount--;
    }

    private int enemiesCount = 0;
    private int sequenceIndex = 0;
    protected int spawnIndex = 0;

    protected static GameObject currentBoy;

    public static List<GameObject> boysList = new List<GameObject>();

    private static RoomLighting roomLighting;
    private static RelodScene scenesController;

    private bool isPointVictory = false;
    public bool IsInfSpawn { get { return isInfSpawn; } }
}
