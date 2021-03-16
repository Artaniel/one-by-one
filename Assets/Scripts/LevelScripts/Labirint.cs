﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Game.Events;

public class RoomBlueprint
{
    public Dictionary<Direction.Side, int> rooms = new Dictionary<Direction.Side, int>();

    public GameObject instance; // link to room if it is spawned
    public GameObject prefab;

    public bool visited = false;
    public string exitSceneName = ""; // not empty only for exit room

    public GameObject contanerPrefab = null;
    public bool containerWasOpened = false;
}

public class Labirint : MonoBehaviour
{
    //public GameObject[] RoomPrefabs;//from inspector 
    public RoomBlueprint[] blueprints; 
    private List<int> activeRooms = new List<int>();
    public int currentRoomID = 0;
    [SerializeField] private float addDistanceToNewDoor = 0; // distance from old door no new door, defines distance between rooms
    static public Labirint instance;
    private Vector3 respawnPoint;
    public string difficultySetting = "1";
    public List<MonsterRoomModifier> commonMRMods;
    [SerializeField] public LocationName locationName = null;
    [HideInInspector] static public Room currentRoom;
    [HideInInspector] public bool OneRoomMode = false;
    public AudioClip music = null;
    public bool saveLocationName = true;

    private void Awake()
    {
        instance = this;
        DifficultyLoad();
        SaveLevelProgressIfNeeded();

        Room.OnAnyRoomEnter = new UnityEngine.Events.UnityEvent();
        Room.OnAnyRoomLeave = new UnityEngine.Events.UnityEvent();
        
        MonsterLife.ClearUsedNames();
        Debug.Log($"Lab. - {SceneManager.GetActiveScene().name}");
        Debug.Log($"Diff. - {SaveLoading.difficulty}. Seed - {SaveLoading.seed}");
    }

    private void Start()
    {
        LabirintBuilder builder = GetComponent<LabirintBuilder>();
        if (builder == null)
        {
            Debug.Log("Cant find labirint builder script. One Room labirint mode");
            blueprints = new RoomBlueprint[1];
            blueprints[0] = new RoomBlueprint();
            OneRoomMode = true;
        }
        else
        {
            builder.BuildLabirint(this);
        }

        StartingRoomSpawn();
        if (locationName)
        {
            if (SaveLoading.sceneNameForCurrentLocation == SceneManager.GetActiveScene().name && saveLocationName)
            {
                EventManager.Notify(SaveLoading.currentLocationName, 1);
            }
            else
            {
                locationName = Instantiate(locationName);
                string locationNameString = locationName.GetRandomName();
                SaveLoading.SaveLocationtName(locationNameString);
                EventManager.Notify(locationNameString, 1);
            }
        }
    }
    
    public void InitBlueprintsFromBuilder() {
        for (int i = 0; i < blueprints.Length; i++)
        {
            blueprints[i] = new RoomBlueprint();
        }
    }

    private void HardcodeLabirintConstruction()
    {

        blueprints[0].rooms[Direction.Side.RIGHT] = 1; // хардкод для связей между комнатами
        blueprints[1].rooms[Direction.Side.LEFT] = 0;

        blueprints[1].rooms[Direction.Side.RIGHT] = 2;
        blueprints[2].rooms[Direction.Side.LEFT] = 1;

        blueprints[1].rooms[Direction.Side.UP] = 3;
        blueprints[3].rooms[Direction.Side.DOWN] = 1;

        //         [3]
        //          |
        //map: [0]-[1]-[2]- -> bossScene
    }

    void StartingRoomSpawn() {
        if (GameObject.FindGameObjectWithTag("Room") == null)
        {
            ActivateRoom(0);
            PreloadRooms();
            OnRoomChanged(0);
            blueprints[0].instance.GetComponent<Room>().ArenaInitCheck();
            blueprints[0].instance.GetComponent<Room>().LightsOn();
        }
        else
        { // for start from choisen room, add prefab, set roomID, and connected room will be spawned
            Room startingRoom = GameObject.FindGameObjectWithTag("Room").GetComponent<Room>();
            CameraForLabirint.instance = GetComponent<CameraForLabirint>();
            startingRoom.roomID = 0;
            activeRooms.Add(0);
            blueprints[0].instance = startingRoom.gameObject;
            startingRoom.GetComponent<RoomLighting>()?.SetSceneLight();
            startingRoom.GetComponent<MonsterManager>()?.Init();
            startingRoom.LightsOn();
            startingRoom.DoorsInit();
            PreloadRooms();
            OnRoomChanged(0);
            startingRoom.DoorsInit(); // да, надо 2 раза. Первый чтобы нашло массив дверей до соединения их с соседями, второй чтобы развешало Locked флаг
            startingRoom.ArenaInitCheck();
        }
    }

    public void OnRoomChanged(int roomIndex){ // spawn neighbors and destroy not neighbor rooms after transition to new room
        currentRoomID = roomIndex;
        List<int> roomsToActivate = new List<int>(); // list of rooms wich should be present after this method 
        roomsToActivate.Add(currentRoomID);
        
        foreach (var side in Direction.sides)
            if (blueprints[currentRoomID].rooms.ContainsKey(side))
                roomsToActivate.Add(blueprints[currentRoomID].rooms[side]);

        //disable rooms who are not neighbirs
        List<int> toRemove = new List<int>();
        foreach (int roomID in activeRooms)
            if (!roomsToActivate.Contains(roomID))
            {                
                blueprints[roomID].instance.SetActive(false);
                toRemove.Add(roomID);
            }
        foreach (int roomID in toRemove) { // because cant remove from list in foreach of same list
            activeRooms.Remove(roomID);
        }

        // add rooms who neighbors and not spawned earlier
        foreach (int roomID in roomsToActivate) {
            if (!activeRooms.Contains(roomID))
            {
                ActivateRoom(roomID);
                Room currentIterationRoom = blueprints[currentRoomID].instance.GetComponent<Room>();
                Room newRoom = blueprints[roomID].instance.GetComponent<Room>();
                Door oldDoor = null;
                Door newDoor = null;
                Vector3 offset = Vector3.zero;
                foreach (var side in Direction.sides)
                {
                    if (blueprints[currentRoomID].rooms.ContainsKey(side) && blueprints[currentRoomID].rooms[side] == roomID)
                    {
                        oldDoor = currentIterationRoom.doorsSided[side];
                        newDoor = newRoom.doorsSided[Direction.InvertSide(side)];
                        oldDoor.SpawnDoor();
                        newDoor.SpawnDoor();
                        //offset = Direction.SideToVector3(side) * distanceToNewDoor;
                        offset = OffsetFromRoomBounds(oldDoor, newDoor, side);
                    }
                }
                ConnectDoors(oldDoor, newDoor);
                offset = oldDoor.transform.localPosition + offset - newDoor.transform.localPosition; // between rooms
                newRoom.transform.position = currentIterationRoom.transform.position + offset;

                if (blueprints[roomID].instance.GetComponent<ArenaEnemySpawner>() != null && roomID != currentRoomID) {
                    //if room with arena, but we are not in it yet
                    blueprints[roomID].instance.GetComponent<ArenaEnemySpawner>().enabled = false;
                }
            }
        }
        if (blueprints[currentRoomID].visited || currentRoomID == 0) {
            if (!CameraForLabirint.instance) 
                CameraForLabirint.instance = GetComponent<CameraForLabirint>();
            if (CameraForLabirint.instance)
                CameraForLabirint.instance.CameraFreeSetup();
            else Debug.LogError("Can't find CameraForLabirint script. Move it to Labirint, or don't use Room prefab spawned from editor.");            
        }
        respawnPoint = GameObject.FindWithTag("Player").transform.position;
        ExitCheck();
        ContainerCheck();
        currentRoom = blueprints[currentRoomID].instance.GetComponent<Room>();
    }

    void ConnectDoors(Door door1, Door door2) {
        door1.connectedDoor = door2;
        door2.connectedDoor = door1;
    }

    void ActivateRoom(int id)
    {
        if (blueprints[id].instance)
            blueprints[id].instance.SetActive(true);
        else SpawnRoom(id);
        activeRooms.Add(id);
    }

    void SpawnRoom(int id)
    {
        blueprints[id].instance = (GameObject)Instantiate(blueprints[id].prefab, Vector3.zero, Quaternion.identity); // zero position to move prefab under player
        blueprints[id].instance.GetComponent<Room>().roomID = id;
        blueprints[id].instance.GetComponent<Room>().DoorsInit();
    }

    public void ReloadRoom() { // сейчас не используется. делалось для перерождения игрока в этой же комнате
        Vector3 savedPosition = blueprints[currentRoomID].instance.transform.position;
        blueprints[currentRoomID].instance.GetComponent<ArenaEnemySpawner>()?.KillThemAll();
        blueprints[currentRoomID].instance.GetComponent<Room>().DisconnectRoom();
        Destroy(blueprints[currentRoomID].instance);
        ActivateRoom(currentRoomID);
        blueprints[currentRoomID].instance.transform.position = savedPosition;

        foreach (var side in Direction.sides)
        {
            if (blueprints[currentRoomID].rooms[side] > -1)
                ConnectDoors(
                    blueprints[currentRoomID].instance.GetComponent<Room>().doorsSided[side], 
                    blueprints[blueprints[currentRoomID].rooms[side]].instance.GetComponent<Room>().doorsSided[Direction.InvertSide(side)]);
        }
        
        GameObject player = GameObject.FindWithTag("Player");
        player.transform.position = respawnPoint;
    }

    void ExitCheck() {
        if (blueprints[currentRoomID].exitSceneName != "")
        {
            foreach (Direction.Side side in Direction.sides)
            {
                if (blueprints[currentRoomID].rooms.ContainsKey(side))
                {
                    Door exitDoor = blueprints[currentRoomID].instance.GetComponent<Room>().doorsSided[Direction.InvertSide(side)];
                    exitDoor.SpawnDoor();
                    exitDoor.sceneName = blueprints[currentRoomID].exitSceneName;
                }
            }
        }
    }

    void ContainerCheck() {
        if (blueprints[currentRoomID].contanerPrefab != null && !blueprints[currentRoomID].containerWasOpened &&
            !blueprints[currentRoomID].instance.GetComponent<Room>().containerAlreadySpawned) {
            Debug.Log("Container check failed");
            GameObject container = Instantiate(blueprints[currentRoomID].contanerPrefab, 
                blueprints[currentRoomID].instance.GetComponent<Room>().possibleContainerPosition.position, Quaternion.identity);
            container.transform.parent = blueprints[currentRoomID].instance.transform;
            container.GetComponent<Container>().blueprint = blueprints[currentRoomID];
            blueprints[currentRoomID].instance.GetComponent<Room>().containerAlreadySpawned = true;
        }
    }

    public static GameObject GetCurrentRoom() {
        return instance ? instance.blueprints[instance.currentRoomID].instance : null;
    }

    private void DifficultyLoad()
    {
        difficultySetting = SaveLoading.difficulty.ToString();
        if (difficultySetting == "1")
        {
            //Debug.Log("Normal mode loaded");
        }
        else if (difficultySetting == "2")
        {
            //Debug.Log("Hard mode loaded");
        }
        else
        {
            Debug.Log("Error on difficulty load, difficultySetting = " + difficultySetting.ToString());
            difficultySetting = "1"; // to avoid errors on user side, better to load wrong difficulty than to crash
        }
    }

    private Vector3 OffsetFromRoomBounds(Door oldDoor, Door newDoor, Direction.Side side) {
        float distanceDoorToBorderOld;
        float distanceDoorToBorderNew;
        if (side == Direction.Side.UP || side == Direction.Side.DOWN)
        {
            distanceDoorToBorderOld = Mathf.Abs(oldDoor.room.GetBordersFromTilemap()[side] - oldDoor.transform.position.y);
            distanceDoorToBorderNew = Mathf.Abs(newDoor.room.GetBordersFromTilemap()[Direction.InvertSide(side)] - newDoor.transform.position.y);
        }
        else
        {
            distanceDoorToBorderOld = Mathf.Abs(oldDoor.room.GetBordersFromTilemap()[side] - oldDoor.transform.position.x);
            distanceDoorToBorderNew = Mathf.Abs(newDoor.room.GetBordersFromTilemap()[Direction.InvertSide(side)] - newDoor.transform.position.x);
        }

        Vector3 result = Direction.SideToVector3(side) * (distanceDoorToBorderOld + distanceDoorToBorderNew);
        float resultMagnitude = result.magnitude;
        result = result.normalized * (resultMagnitude + addDistanceToNewDoor);

        return result;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.LeftControl)) // ctrl+R => reboot
            SceneLoading.LoadScene(SceneManager.GetActiveScene().name, true);

        if (CharacterLife.isDeath && Input.GetKeyDown(KeyCode.R)) // death && R => reboot
            SceneLoading.LoadScene(SceneManager.GetActiveScene().name, true);
    }

    private void SaveLevelProgressIfNeeded()
    {
        string currentSceneNumber = SceneManager.GetActiveScene().name;
        if (SaveLoading.currentScene != currentSceneNumber)
        {            
            SaveLoading.SaveCurrentScene(SceneManager.GetActiveScene().name);
        }
    }

    private void PreloadRooms() {
        if (blueprints[0].instance)
        {
            List<int> growZoneRoomsIDs = new List<int>();
            growZoneRoomsIDs.Add(0);
            while (growZoneRoomsIDs.Count>0) {
                foreach (Direction.Side side in Direction.sides)
                {
                    if (blueprints[growZoneRoomsIDs[0]].rooms.ContainsKey(side)) // if next room should exist
                        if (blueprints[blueprints[growZoneRoomsIDs[0]].rooms[side]].instance == null) // if it does not exist
                        {
                            SpawnRoom(blueprints[growZoneRoomsIDs[0]].rooms[side]);
                            Door oldDoor = blueprints[growZoneRoomsIDs[0]].instance.GetComponent<Room>().doorsSided[side];
                            Door newDoor = blueprints[blueprints[growZoneRoomsIDs[0]].rooms[side]].instance.GetComponent<Room>().doorsSided[Direction.InvertSide(side)];
                            oldDoor.SpawnDoor();
                            newDoor.SpawnDoor();
                            ConnectDoors(oldDoor, newDoor);                            
                            Vector3 offset = oldDoor.transform.localPosition + OffsetFromRoomBounds(oldDoor, newDoor, side) - newDoor.transform.localPosition; 
                            blueprints[blueprints[growZoneRoomsIDs[0]].rooms[side]].instance.transform.position = blueprints[growZoneRoomsIDs[0]].instance.transform.position + offset; // set position for new room
                            growZoneRoomsIDs.Add(blueprints[growZoneRoomsIDs[0]].rooms[side]);
                            activeRooms.Add(blueprints[growZoneRoomsIDs[0]].rooms[side]); // leave active to prevent errors on tilemap calculations for neighbor rooms
                        }
                }
                if (blueprints[growZoneRoomsIDs[0]].contanerPrefab != null
                    && !blueprints[growZoneRoomsIDs[0]].instance.GetComponent<Room>().containerAlreadySpawned)
                {
                    GameObject container = Instantiate(blueprints[growZoneRoomsIDs[0]].contanerPrefab,
                        blueprints[growZoneRoomsIDs[0]].instance.GetComponent<Room>().possibleContainerPosition.position, Quaternion.identity);
                    container.transform.parent = blueprints[growZoneRoomsIDs[0]].instance.transform;
                    container.GetComponent<Container>().blueprint = blueprints[growZoneRoomsIDs[0]];
                    blueprints[growZoneRoomsIDs[0]].instance.GetComponent<Room>().containerAlreadySpawned = true;
                }

                growZoneRoomsIDs.Remove(growZoneRoomsIDs[0]);
            }
        }
        else Debug.LogError("labirint preload error. Didn't get 0 room");        
    }
}
