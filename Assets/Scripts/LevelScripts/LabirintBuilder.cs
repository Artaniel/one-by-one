using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LabirintBuilder : MonoBehaviour
{
    [SerializeField]
    private int numberOfRooms = 10;
    [SerializeField]
    private int correctPathLength = 3;

    [SerializeField]
    private GameObject[] peacefulRoomPrefabs = null;
    [SerializeField]
    private string exitSceneName = "";
    [SerializeField]
    private GameObject[] containersPrefabs = null;
    [SerializeField]
    private bool roomRepeatAllowed = false;
    [SerializeField]
    private int treasureRoomsNumber = 0;
    [SerializeField]
    private GameObject[] treasureRoomPrefabs = null;

    private Labirint labirint;
    private int[,] map; //room position to room id
    public Dictionary<int, Vector2Int> allRoomsPositions { get; private set; } // room id to room position
    private List<Vector2Int> correctPathRoomsPositions;
    private Vector2Int startPosition;
    private Vector2Int endPosition;
    private Vector2Int currentPosition;
    private int lastRoomID;

    static public string seed = "";

    public int combatRoomListSize = -1;
    [HideInInspector] public GameObject[] combatRoomPrefabs = null;
    [HideInInspector] public float[] combatRoomChances = new float[0];

    private void Init()
    {
        map = new int[2 * numberOfRooms, 2 * numberOfRooms];
        for (int i = 0; i < 2 * numberOfRooms; i++)
            for (int j = 0; j < 2 * numberOfRooms; j++)
            {
                map[i, j] = -1;
            }
        if (combatRoomListSize == -1)
            SetCombatRoomCountFromArraySize();
    }

    public void BuildLabirint(Labirint labirintScript)
    {
        labirint = labirintScript;
        Init();
        labirint.blueprints = new RoomBlueprint[numberOfRooms];
        labirint.InitBlueprintsFromBuilder();
        startPosition = new Vector2Int(numberOfRooms, numberOfRooms); // середина
        currentPosition = startPosition;
        allRoomsPositions = new Dictionary<int, Vector2Int>();
        allRoomsPositions.Add(0, startPosition);
        correctPathRoomsPositions = new List<Vector2Int>();
        correctPathRoomsPositions.Add(startPosition);
        map[startPosition.x, startPosition.y] = 0;
        lastRoomID = 0;

        SeedCheck();

        MakeCorrectPath();
        MakeDeadEnds();
        FillRoomPrefabs();
        //FillContainers();
        FillTreasureRooms();
        //DrawMap();
    }

    private void MakeCorrectPath()
    {
        Vector2Int newPosition;
        Vector2Int positionToMove;
        List<Direction.Side> availableSides;
        while (lastRoomID < correctPathLength - 1)
        {
            availableSides = new List<Direction.Side>();
            foreach (Direction.Side side in Direction.sides)
            {
                positionToMove = currentPosition + Direction.SideToVector2Int(side);
                if (map[positionToMove.x, positionToMove.y] == -1)
                    availableSides.Add(side);
            }
            if (availableSides.Count == 0) // if dead end
            {
                correctPathRoomsPositions.Remove(currentPosition);
                StepBack();
            }
            else
            {
                Direction.Side stepDirrection = availableSides[Random.Range(0, availableSides.Count)]; // random available side
                newPosition = currentPosition + Direction.SideToVector2Int(stepDirrection);
                lastRoomID++;
                map[newPosition.x, newPosition.y] = lastRoomID;
                ConnectRoomBlueprints(map[currentPosition.x, currentPosition.y], map[newPosition.x, newPosition.y], stepDirrection);
                allRoomsPositions.Add(lastRoomID, newPosition);
                correctPathRoomsPositions.Add(newPosition);
                currentPosition = newPosition;
            }
        }
        endPosition = currentPosition;
    }

    private void StepBack()
    {
        Vector2Int positionToMove = currentPosition;
        int indexMax = -1;
        foreach (Direction.Side side in Direction.sides)
        {
            Vector2Int newPosition = currentPosition + Direction.SideToVector2Int(side);
            if ((map[newPosition.x, newPosition.y] > indexMax) && correctPathRoomsPositions.Contains(newPosition))
            {
                indexMax = map[newPosition.x, newPosition.y];
                positionToMove = newPosition;
            }
        }
        currentPosition = positionToMove;
    }

    private void MakeDeadEnds()
    {
        Vector2Int randomRoomPosition;
        List<Direction.Side> availableSides;
        List<Vector2Int> roomsWithPossibleExitPositions = new List<Vector2Int>(correctPathRoomsPositions);
        roomsWithPossibleExitPositions.Remove(endPosition);
        Vector2Int newPosition;
        Vector2Int positionToMove;
        while (lastRoomID < numberOfRooms - 1)
        {
            randomRoomPosition = roomsWithPossibleExitPositions[Random.Range(0, roomsWithPossibleExitPositions.Count)];
            availableSides = new List<Direction.Side>();
            foreach (Direction.Side side in Direction.sides)
            {
                positionToMove = randomRoomPosition + Direction.SideToVector2Int(side);
                if (map[positionToMove.x, positionToMove.y] == -1)
                    availableSides.Add(side);
            }
            if (availableSides.Count != 0)
            {
                Direction.Side randomSide = availableSides[Random.Range(0, availableSides.Count)];
                newPosition = randomRoomPosition + Direction.SideToVector2Int(randomSide);
                lastRoomID++;
                map[newPosition.x, newPosition.y] = lastRoomID;
                ConnectRoomBlueprints(map[randomRoomPosition.x, randomRoomPosition.y], lastRoomID, randomSide);
                allRoomsPositions.Add(lastRoomID, newPosition);
                roomsWithPossibleExitPositions.Add(newPosition);
            }
            else
            {
                roomsWithPossibleExitPositions.Remove(randomRoomPosition);
            }
        }
    }

    private void DrawMap()
    {// for debug only
        Color lineColor;
        for (int i = 0; i < allRoomsPositions.Count; i++)
        {
            Vector3 playerPosition = GameObject.FindWithTag("Player").transform.position;
            if (correctPathRoomsPositions.Contains(allRoomsPositions[i]))
                lineColor = Color.green;
            else
                lineColor = Color.red;
            foreach (Direction.Side side in Direction.sides)
            {
                if (Labirint.instance.blueprints[i].rooms.ContainsKey(side))
                    if (Labirint.instance.blueprints[i].rooms[side] != -1)
                    {
                        Debug.DrawRay(playerPosition + new Vector3(allRoomsPositions[i].x - numberOfRooms + 0.5f, allRoomsPositions[i].y - numberOfRooms + 0.5f, 0),
                            Direction.SideToVector3(side), lineColor, 5f);
                    }
            }
            if (Labirint.instance.blueprints[i].contanerPrefab != null)
            {
                lineColor = Color.blue;
                Vector3 point = playerPosition + new Vector3(allRoomsPositions[i].x - numberOfRooms + 0.5f, allRoomsPositions[i].y - numberOfRooms + 0.5f, 0);
                Debug.DrawLine(point + (0.25f * Vector3.up) + (0.25f * Vector3.left), point + (0.25f * Vector3.down) + (0.25f * Vector3.right), lineColor, 5f);
                Debug.DrawLine(point + (0.25f * Vector3.down) + (0.25f * Vector3.left), point + (0.25f * Vector3.up) + (0.25f * Vector3.right), lineColor, 5f);
            }
        }
    }

    private void ConnectRoomBlueprints(int firstID, int secondID, Direction.Side direction1to2)
    {
        Labirint.instance.blueprints[firstID].rooms[direction1to2] = secondID;
        Labirint.instance.blueprints[secondID].rooms[Direction.InvertSide(direction1to2)] = firstID;
    }

    private void FillRoomPrefabs()
    {
        List<GameObject> emptyRoomsList = new List<GameObject>(peacefulRoomPrefabs);
        //List<GameObject> combatRoomsList = new List<GameObject>(combatRoomPrefabs);
        labirint.blueprints[0].prefab = RandomGameObjectFromList(emptyRoomsList); // 0 index is for starting room, always empty
        if (!roomRepeatAllowed) emptyRoomsList.Remove(labirint.blueprints[0].prefab);
        if (combatRoomChances == null || combatRoomChances.Length == 0) {
            combatRoomChances = new float[combatRoomPrefabs.Length];
            for (int i = 0; i < combatRoomPrefabs.Length; i++)
                combatRoomChances[i] = 10;
        }
        for (int i = 1; i < numberOfRooms; i++)
        {
            if (allRoomsPositions[i] != endPosition)
            {
                if (combatRoomChances != null)
                {
                    labirint.blueprints[i].prefab = RandomGameObjectFromList(combatRoomPrefabs, combatRoomChances, combatRoomListSize, !roomRepeatAllowed);
                    if (!roomRepeatAllowed) combatRoomListSize--;
                }
            }
            else
            {
                labirint.blueprints[i].prefab = RandomGameObjectFromList(emptyRoomsList);
                if (!roomRepeatAllowed) emptyRoomsList.Remove(labirint.blueprints[i].prefab);
                labirint.blueprints[i].exitSceneName = exitSceneName;
            }
        }
        labirint.blueprints[map[endPosition.x, endPosition.y]].exitSceneName = exitSceneName;
    }

    private void FillContainers()
    {
        List<int> containerAvailableRooms = new List<int>(allRoomsPositions.Keys);
        containerAvailableRooms.Remove(0);                                  // no containers in first room
        containerAvailableRooms.Remove(map[endPosition.x, endPosition.y]);  // and last room

        if (containerAvailableRooms.Count < containersPrefabs.Length)
            Debug.LogError("not enough rooms for containtes");
        else
            foreach (GameObject containerPrefab in containersPrefabs)
            {
                int roomForContainerID = containerAvailableRooms[Random.Range(0, containerAvailableRooms.Count - 1)];
                labirint.blueprints[roomForContainerID].contanerPrefab = containerPrefab;
                containerAvailableRooms.Remove(roomForContainerID);
            }
    }

    private GameObject RandomGameObjectFromList(List<GameObject> prefabList)
    {
        if (prefabList.Count == 0)
        {
            Debug.LogError("Not enough room prefabs to fill rooms to labirintBuilder");
            return null;
        }
        return prefabList[Random.Range(0, prefabList.Count)];
    }

    private GameObject RandomGameObjectFromList(GameObject[] prefabList, float[] chancesList, int listLimiter, bool removeFromList)
    {
        if (prefabList.Length == 0)
        {
            Debug.LogError("Not enough room prefabs to fill rooms to labirintBuilder");
            return null;
        }
        GameObject result = null;

        float summ = 0;
        for (int i=0; i < listLimiter; i++)
        {
            summ += chancesList[i];
        }
        if (summ > 0) // exception for 0 chances for all items
        {
            float random = Random.Range(0f, summ);
            int i = 0;
            while (random > 0)
            {
                random -= chancesList[i];
                i++;
            }
            result = prefabList[i - 1];

            if (removeFromList)
            {
                while (i < listLimiter)
                {
                    prefabList[i - 1] = prefabList[i];
                    chancesList[i - 1] = chancesList[i];
                    i++;
                }
            }
        }

        return result;
    }

    private void FillTreasureRooms()
    {
        List<int> containerAvailableRooms = new List<int>(allRoomsPositions.Keys);
        containerAvailableRooms.Remove(0);                                  // no containers in first room
        containerAvailableRooms.Remove(map[endPosition.x, endPosition.y]);  // and last room
        List<GameObject> containerList = new List<GameObject>(containersPrefabs);
        List<GameObject> trasureRoomPrefabsList = new List<GameObject>(treasureRoomPrefabs);
        bool repeatTreasureRoomPrefabs = (trasureRoomPrefabsList.Count < treasureRoomsNumber);
        bool repeatContainers = (containerList.Count < treasureRoomsNumber);

        List<int> deadEnds = new List<int>();
        List<int> wrongWayNotDeadEnds = new List<int>();
        foreach (int roomID in containerAvailableRooms)
        {
            int exits = 0;
            foreach (Direction.Side side in Direction.sides)
                if (Labirint.instance.blueprints[roomID].rooms.ContainsKey(side)) exits++;
            if (exits == 1)
                deadEnds.Add(roomID);
            else if (!correctPathRoomsPositions.Contains(allRoomsPositions[roomID]))
                wrongWayNotDeadEnds.Add(roomID);
        }
        if (deadEnds.Count >= treasureRoomsNumber)
            containerAvailableRooms = new List<int>(deadEnds);
        else if (deadEnds.Count + wrongWayNotDeadEnds.Count >= treasureRoomsNumber)
        {
            containerAvailableRooms = new List<int>(deadEnds);
            foreach (int roomID in wrongWayNotDeadEnds)
            {
                containerAvailableRooms.Add(roomID);
            }
        }

        for (int i = 0; i < treasureRoomsNumber; i++)
            //foreach (GameObject trsureRoomPrefab in treasureRoomPrefabs) {
            if (containerAvailableRooms.Count <= 0)
                Debug.LogError("not enough rooms for containtes");
            else
            {
                GameObject trsureRoomPrefab = trasureRoomPrefabsList[Random.Range(0, trasureRoomPrefabsList.Count)];
                int randomRoomID = containerAvailableRooms[Random.Range(0, containerAvailableRooms.Count)];
                GameObject randomContainer = containerList[Random.Range(0, containerList.Count)];
                labirint.blueprints[randomRoomID].prefab = trsureRoomPrefab;
                labirint.blueprints[randomRoomID].contanerPrefab = randomContainer;
                if (!repeatContainers) containerList.Remove(randomContainer);
                if (!repeatTreasureRoomPrefabs) trasureRoomPrefabsList.Remove(trsureRoomPrefab);
                containerAvailableRooms.Remove(randomRoomID); // to prevent 2 treasure room placement in same room
            }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) && (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))) // Alt+M => DrawMap
            DrawMap();
    }

    private void SeedCheck()
    {
        seed = SaveLoading.seed;
        if (seed != "")
        {
            Random.InitState(seed.GetHashCode() + SceneManager.GetActiveScene().name.GetHashCode());
            Debug.Log("seed = " + seed);
        }
        else
        {
            seed = Random.Range(0, int.MaxValue).ToString();
            Random.InitState(seed.GetHashCode() + SceneManager.GetActiveScene().name.GetHashCode());
            Debug.Log("temp seed = " + seed);
        }
    }

    static public void SetupSeed(string seedInput)
    {
        seed = seedInput;
        SaveLoading.SaveSeed(seed);
    }

    static public void ResetSeed()
    {
        seed = "";
        SaveLoading.SaveSeed("");
    }

    private void SetCombatRoomCountFromArraySize() {
        int arraySize = 0;
        for (int i = 0; i < combatRoomPrefabs.Length; i++)
            if (combatRoomPrefabs != null)
                arraySize++;
        combatRoomListSize = arraySize;
    }

#if UNITY_EDITOR
    public static void Table(LabirintBuilder builderScript) // for inspector UI
    {
        if (builderScript.combatRoomListSize == -1)
            builderScript.SetCombatRoomCountFromArraySize();
        if (builderScript.combatRoomListSize > 100) builderScript.combatRoomListSize = 100; // to prevent huge arrays
        GUILayout.BeginHorizontal(); // table headline
        GUILayout.Label("Prefab", GUILayout.Width(120));
        GUILayout.Label("Chance", GUILayout.Width(50));
        GUILayout.Label("%", GUILayout.Width(50));
        GUILayout.EndHorizontal();

        if (builderScript.combatRoomChances == null || builderScript.combatRoomChances.Length == 0) {
            builderScript.combatRoomChances = new float[builderScript.combatRoomPrefabs.Length];
            for (int i = 0; i < builderScript.combatRoomChances.Length; i++)            
                builderScript.combatRoomChances[i] = 10;            
        }

        float[] lastChances = new float[builderScript.combatRoomChances.Length];
        for (int i = 0; i < lastChances.Length; i++) 
            lastChances[i] = builderScript.combatRoomChances[i];

        GameObject[] lastPrefabList = new GameObject[builderScript.combatRoomPrefabs.Length];
        for (int i = 0; i < lastPrefabList.Length; i++)
            lastPrefabList[i] = builderScript.combatRoomPrefabs[i];

        if (lastChances.Length < builderScript.combatRoomListSize)
        { //if array size added, make new arrays and move data to them   
            builderScript.combatRoomChances = new float[Mathf.Max(builderScript.combatRoomListSize, builderScript.combatRoomChances.Length)];
            builderScript.combatRoomPrefabs = new GameObject[Mathf.Max(builderScript.combatRoomListSize, builderScript.combatRoomPrefabs.Length)];
            for (int i = 0; i < Mathf.Min(lastPrefabList.Length, builderScript.combatRoomPrefabs.Length); i++)
            {
                builderScript.combatRoomChances[i] = lastChances[i];
                builderScript.combatRoomPrefabs[i] = lastPrefabList[i];
            }
            EditorUtility.SetDirty(builderScript); // to prevent load from prefab on Play    
        }
        if (lastChances.Length > builderScript.combatRoomListSize) {
            EditorUtility.SetDirty(builderScript);
        }

        float summ = 0; // summ for % output
        for (int i = 0; i < builderScript.combatRoomListSize; i++)
        {
            summ += builderScript.combatRoomChances[i];
        }
        for (int i = 0; i < builderScript.combatRoomListSize; i++)
        {
            GUILayout.BeginHorizontal();
            {
                GameObject lastPrefab = builderScript.combatRoomPrefabs[i];
                builderScript.combatRoomPrefabs[i] = (GameObject)EditorGUILayout.ObjectField(builderScript.combatRoomPrefabs[i], typeof(GameObject), false, GUILayout.Width(120));
                if (lastPrefab != builderScript.combatRoomPrefabs[i])
                {
                    EditorUtility.SetDirty(builderScript);
                    if (builderScript.combatRoomPrefabs[i] != null && builderScript.combatRoomChances[i] == 0)
                        builderScript.combatRoomChances[i] = 10;
                }
                string chance = builderScript.combatRoomChances[i].ToString();
                chance = EditorGUILayout.TextField("", chance, GUILayout.Width(50));
                if (chance != builderScript.combatRoomChances[i].ToString()) // if changed
                    EditorUtility.SetDirty(builderScript);
                float.TryParse(chance, out builderScript.combatRoomChances[i]);

                EditorGUILayout.LabelField((builderScript.combatRoomChances[i] * 100f / summ).ToString() + "%");
            }
            GUILayout.EndHorizontal();
        }
    }
#endif
}
