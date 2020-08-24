using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class FireOnTilemap : MonoBehaviour
{
    public Room room;
    private int[,] fireMap; // 2,3 - default. 1 - wall. 4 - active fire. 5 - ended fire. 6 - highly flamable;
    [SerializeField] private GameObject firePrefab;
    [SerializeField] private float extinguishProbability = 0.015f; //Rd, 
    private float extinguishCheckPeriod = 0.25f;
    [SerializeField] private float spreadProbability = 0.25f; //Rm
    [SerializeField] private float spreadCheckPeriod = 0.1f; //Tm
    private float extinguishTimer, spreadTimer;
    static private GameObject firePrefabStatic = null;
    static public bool damageMobsAllowed = false;

    private List<GameObject> activeFires;
    private Vector3Int arrayToTilemap;
    private List<GameObject> trees;

    public bool dryRoom = false;
    public bool cleanedRoom = false;

    private GameObject player;
    private CharacterLife characterLife;
    private CurrentEnemySelector currentEnemySelector;

    private void Awake()
    {
        if (!room) room = GetComponent<Room>();
        if (!room) Debug.LogError("Fire can't find Room script");
        player = GameObject.FindWithTag("Player");
        characterLife = player.GetComponent<CharacterLife>();
        currentEnemySelector = Labirint.instance.GetComponent<CurrentEnemySelector>();
        Init();
    }

    private void Start()
    {
        //включить для инициализации теста
        //Test();
    }

    private void Test()
    {
        StartFireInternal(GameObject.FindWithTag("Player").transform.position); // огонь под игроком
        FlamableRect(GameObject.FindWithTag("Player").transform.position.x + 5, GameObject.FindWithTag("Player").transform.position.y + 5, 5, 5); // зажигательная область 
        Debug.DrawLine(GameObject.FindWithTag("Player").transform.position + new Vector3(5, 5, 0), 
            GameObject.FindWithTag("Player").transform.position + new Vector3(10, 10, 0), Color.red, 999f); // и отладочная линия чтобы было видно где эта область
    }

    static public void StartFire(Vector2 firePosition, GameObject firePrefab) {
        if (!firePrefabStatic) firePrefabStatic = firePrefab;
        if (!Labirint.GetCurrentRoom().GetComponent<FireOnTilemap>())
        {
            FireOnTilemap currentScript = Labirint.GetCurrentRoom().AddComponent<FireOnTilemap>();
            currentScript.room = Labirint.currentRoom;
            Labirint.GetCurrentRoom().GetComponent<Room>().fireScript = currentScript;
            currentScript.firePrefab = firePrefab;
            currentScript.Init();
            currentScript.StartFireInternal(firePosition);
        }
        else {
            Labirint.GetCurrentRoom().GetComponent<FireOnTilemap>().StartFireInternal(firePosition);
        }
    }

    private void Init()
    {
        room.fireScript = this;
        if (room.OOBmap == null) room.FillOOB();
        fireMap = room.OOBmap;
        activeFires = new List<GameObject>();
        extinguishTimer = extinguishCheckPeriod;
        spreadTimer = spreadCheckPeriod;
        arrayToTilemap = new Vector3Int(room.leftBorder, room.botBorder,0);
        trees = new List<GameObject>();
        foreach (GameObject tree in GameObject.FindGameObjectsWithTag("Environment"))
        {
            if (tree.name.Contains("tree") || tree.name.Contains("Tree") || tree.name.Contains("Bush") || tree.name.Contains("bush"))
            {
                if (room.RectIsInbounds(tree.transform.position.x, tree.transform.position.y, 0, 0)) // if in this room                
                    trees.Add(tree);
            }
        }
        if (TryGetComponent<MonsterManager>(out MonsterManager monsterManager))
        {
            if (monsterManager.EnemyCount() == 0)
            { // exception for spawn fire script on last monster in room
                cleanedRoom = true;
            }
        }

        damageMobsAllowed = false;
        foreach (SkillBase skill in player.GetComponent<SkillManager>().skills) {
            if (skill is FireDamageOnMonsters)
                damageMobsAllowed = true;
        }

        if (dryRoom)
        {
            ppVolume = GetComponent<PostProcessVolume>();
        }
    }

    private void StartFireInternal(Vector2 firePosition) {
        Vector3Int tilemapPosition = room.wallsTilemap.WorldToCell(firePosition) - arrayToTilemap;
        int fireStatusIndex = fireMap[tilemapPosition.x, tilemapPosition.y];
        if (fireStatusIndex == 2 || fireStatusIndex == 3 || fireStatusIndex == 6) // if default state or flamable
        {
            fireMap[tilemapPosition.x, tilemapPosition.y] = 4;
            GameObject newFire = PoolManager.GetPool(firePrefab, room.wallsTilemap.CellToWorld(tilemapPosition + arrayToTilemap), Quaternion.identity);
            newFire.transform.parent = room.transform;
            activeFires.Add(newFire);
            foreach (GameObject tree in trees) 
                if ((room.wallsTilemap.WorldToCell(tree.transform.position) - arrayToTilemap) == tilemapPosition) 
                    LitTree(tree);
        }
    }

    private void LitTree(GameObject tree) { // сюда надо поместить поведение для подожженных деревьев
        if (tree.TryGetComponent<SpriteRenderer>(out SpriteRenderer sprite))
            sprite.color = Color.red;
    }

    static public void EndFire(GameObject fireObject) {
        if (!Labirint.GetCurrentRoom().TryGetComponent<FireOnTilemap>(out FireOnTilemap fireScript))
            Debug.LogError("Error on EndFire, can't find fire manager");
        else
            fireScript.EndFireInternal(fireObject, true);
    }

    private void EndFireInternal(GameObject fireObject, bool returnCellToDelault) {
        Vector3Int tilemapPosition = room.wallsTilemap.WorldToCell(fireObject.transform.position) - arrayToTilemap;
        fireMap[tilemapPosition.x, tilemapPosition.y] = returnCellToDelault ? 2 : 5;
        activeFires.Remove(fireObject);
        var spriteRend = fireObject.GetComponent<SpriteRenderer>();
        spriteRend.sortingOrder = 1;
        var fireMat = spriteRend.material;
        fireMat.SetFloat("_TimeParameter", 0);
        //PoolManager.ReturnToPool(fireObject);
        foreach (GameObject tree in trees)
            if ((room.wallsTilemap.WorldToCell(tree.transform.position) - arrayToTilemap) == tilemapPosition)
                BurnOutTree(tree);
    }

    private void BurnOutTree(GameObject tree) { // сюда логику выгорания деревьев
        if (tree.TryGetComponent<SpriteRenderer>(out SpriteRenderer sprite))
            sprite.color = Color.black;
    }

    private void Update()
    {
        if (!Pause.Paused && Labirint.currentRoom == room)
        {
            ExtinguishTimerTick();
            SpreadTimerCheck();
            PlayerDamageCheck();
            if (damageMobsAllowed)
                DamageMobs();
        }
        if (dryRoom) UpdateDryVFX();
    }

    private void ExtinguishTimerTick() {
        extinguishTimer -= Time.deltaTime;
        if (extinguishTimer < 0) {
            ExtinguishChecks();
            extinguishTimer += extinguishCheckPeriod;
        }
    }

    private void ExtinguishChecks() {
        float currentExtinguishProbability = extinguishProbability;
        if (dryRoom) currentExtinguishProbability /= 2f;
        if (cleanedRoom) currentExtinguishProbability *= 10f;
        if (activeFires.Count > 0)
            for (int i = activeFires.Count - 1; i >= 0; i--)
                if (Random.Range(0f, 1f) <= currentExtinguishProbability)
                    EndFireInternal(activeFires[i], false);
    }

    private void SpreadTimerCheck() {
        spreadTimer -= Time.deltaTime;
        if (spreadTimer < 0) {
            SpreadCheck();
            spreadTimer += spreadCheckPeriod;
        }
    }

    private void SpreadCheck() {
        float currentSpreadProbability = spreadProbability;
        if (cleanedRoom) currentSpreadProbability /= 10f;
        if (dryRoom) currentSpreadProbability *= 2f;
        if (activeFires.Count > 0)
            if (Random.Range(0f, 1f) <= currentSpreadProbability) {
            List<Vector3Int> fireSpreadPossiblePositions = new List<Vector3Int>();
            List<Vector3Int> flamableCellsPositions = new List<Vector3Int>();
            Vector3Int currentFlamePosition, testedPosition; 
            foreach (GameObject flame in activeFires) {
                currentFlamePosition = room.wallsTilemap.WorldToCell(flame.transform.position) - arrayToTilemap;
                foreach (Vector3Int side in Direction.eightDirectionsVectors) {
                    testedPosition = currentFlamePosition + side;
                    if (fireMap[testedPosition.x, testedPosition.y] == 6) // flamable
                        flamableCellsPositions.Add(testedPosition);
                    if (fireMap[testedPosition.x, testedPosition.y] == 2 || (fireMap[testedPosition.x, testedPosition.y] == 3)) {
                        fireSpreadPossiblePositions.Add(testedPosition);
                    }
                }
            }
            if (flamableCellsPositions.Count > 0)
            {
                StartFireInternal(room.wallsTilemap.CellToWorld(flamableCellsPositions[Random.Range(0, flamableCellsPositions.Count)] + arrayToTilemap));
            }
            else if(fireSpreadPossiblePositions.Count > 0) {
                StartFireInternal(room.wallsTilemap.CellToWorld(fireSpreadPossiblePositions[Random.Range(0, fireSpreadPossiblePositions.Count - 1)]+ arrayToTilemap));
            }
        }
    }

    public void FlamableRect(float x, float y, float sizeX, float sizeY) {
        Vector3Int botLeftCornerOnArray = room.wallsTilemap.WorldToCell(new Vector3(x, y, 0)) - arrayToTilemap;
        Vector3Int topRightCornerOnArray = room.wallsTilemap.WorldToCell(new Vector3(x + sizeX, y + sizeY, 0)) - arrayToTilemap;
        for (int i = botLeftCornerOnArray.y; i <= topRightCornerOnArray.y; i++) {
            for (int j = botLeftCornerOnArray.x; j <= topRightCornerOnArray.x; j++) {
                if (fireMap[j, i] == 2 || fireMap[j, i] == 3) // if was default
                    fireMap[j, i] = 6; // change to flamable
            }
        }
    }

    private void PlayerDamageCheck() {
        Vector3Int testedPosition = room.wallsTilemap.WorldToCell(player.transform.position) - arrayToTilemap;
        if (fireMap[testedPosition.x, testedPosition.y] == 4)  // if on tile with fire
            characterLife.Damage(1);        
    }

    private void DamageMobs()
    {
        GameObject currentBoy = currentEnemySelector.currentBoy;
        if (currentBoy)
        {
            Vector3Int testedPosition = room.wallsTilemap.WorldToCell(currentBoy.transform.position) - arrayToTilemap;
            if (fireMap[testedPosition.x, testedPosition.y] == 4) // if on tile with fire
                currentBoy.GetComponent<MonsterLife>().Damage(gameObject, 1);
        }
    }

    private void UpdateDryVFX()
    {
        if (Labirint.currentRoom == room)
            ppVolume.weight = Mathf.Clamp01(ppVolume.weight + Time.deltaTime);
        else
            ppVolume.weight = Mathf.Clamp01(ppVolume.weight - Time.deltaTime);
    }

    private PostProcessVolume ppVolume;
}
