using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

public class Room : MonoBehaviour
{
    private Door[] doors;
    public Dictionary<Direction.Side, Door> doorsSided = new Dictionary<Direction.Side, Door>();

    [HideInInspector]
    private Labirint labirint = null;
    public int roomID = -1; // -1 for not set

    public enum RoomType {empty,arena }
    public RoomType roomType;

    public Transform possibleContainerPosition;
    public bool containerAlreadySpawned = false;

    [HideInInspector] public bool cleared { get; private set; } = false;
    [HideInInspector] public MonsterManager monsterManager;
    [HideInInspector] public List<MonsterRoomModifier> externalMRMods = new List<MonsterRoomModifier>();
    public FireOnTilemap fireScript;

    public static UnityEvent OnAnyRoomEnter = new UnityEvent();
    public static UnityEvent OnAnyRoomLeave = new UnityEvent();
    public UnityEvent OnThisEnter = new UnityEvent();
    public UnityEvent OnThisLeave = new UnityEvent();
    public UnityEvent OnThisClear = new UnityEvent();

    private void Awake()
    {
        if (possibleContainerPosition == null) possibleContainerPosition = transform; // if forgot to set, center of room
        DoorsInit();
        labirint = Labirint.instance;
        externalMRMods = labirint.commonMRMods;
    }

    private void Start()
    {
        FillOOB();
    }

    public void DoorsInit() {
        doors = gameObject.GetComponentsInChildren<Door>();
        foreach (Door door in doors)
        {
            if (door.sceneName == "") {
                if (door.direction == Direction.Side.UNSET && door.directionAutoset())
                     Debug.Log("Door direction was not set");
                else doorsSided[door.direction] = door;
                if (door.room == null) door.room = this;
            }
        }
    }

    public void MoveToRoom(Door wayInDoor) {
        wayInDoor.connectedDoor.room.LeaveRoom();
        CameraForLabirint.instance.ChangeRoom(wayInDoor.room.gameObject, roomType == RoomType.arena && !labirint.blueprints[roomID].visited);

        LightsOn();

        Labirint.instance.OnRoomChanged(roomID);

        var player = GameObject.FindGameObjectWithTag("Player");
        var playerLife = player.GetComponent<CharacterLife>();
        playerLife.HidePlayer();
        var dummy = Instantiate(
            playerLife.dummyPlayerPrefab,
            wayInDoor.transform.position + (4 * Direction.SideToVector3(wayInDoor.direction)),
            Quaternion.identity);
        dummy.GetComponent<DummyPlayerController>().SetDestination(wayInDoor.transform.position);

        var playerMove = player.GetComponent<CharacterMovement>();
        playerMove.enabled = false;
        player.transform.position = wayInDoor.transform.position;

        StartCoroutine(DelayedEnterRoom(player, dummy, wayInDoor.transform.position));
    }

    private IEnumerator DelayedEnterRoom(GameObject player, GameObject dummy, Vector3 position)
    {
        yield return new WaitForSeconds(0.35f);
        
        ArenaInitCheck();
        
        Destroy(dummy);
        player.transform.position = position;
        player.GetComponent<CharacterLife>().RevealPlayer();

        var playerMove = player.GetComponent<CharacterMovement>();
        playerMove.enabled = true;

        OnAnyRoomEnter.Invoke();
        OnThisEnter.Invoke();
    }

    public void ArenaInitCheck()
    {
        if (roomType == RoomType.arena)
        {
            if (!Labirint.instance.blueprints[roomID].visited)
            {
                if (GetComponent<ArenaEnemySpawner>()!=null)
                    GetComponent<ArenaEnemySpawner>().enabled = true;
                if (GetComponent<MonsterManager>() != null)
                    GetComponent<MonsterManager>().UnfreezeMonsters();
                LockRoom();
            }
            else {
                if (GetComponent<ArenaEnemySpawner>() != null)
                    GetComponent<ArenaEnemySpawner>().KillThemAll();
                TimerUnlockRoom();
            }
        }
        else
        {
            TimerUnlockRoom();
        }
    }

    public void DisconnectRoom() // cutting door connections before destroy room, to avoid errors on Door.connectedDoor from neighors to destroyed room
    {
        foreach (Door door in doors) {
            if (door.connectedDoor != null) {
                door.connectedDoor.connectedDoor = null;
                door.connectedDoor = null;
            }
        }
    }

    public void LockRoom()
    {
        if (doors!=null)
            foreach (Door door in doors)
            {
                door.Lock();
            }
    }

    public void TimerUnlockRoom() {
        cleared = true;
        foreach (Door door in doors)
        {
            door.unlockOnTimer = true;
            door.Lock();
        }
        CameraForLabirint.instance.CameraFreeSetup();
        OnThisClear.Invoke();
    }

    public void LeaveRoom() {
        if (roomType == RoomType.arena)
        {
            GetComponent<ArenaEnemySpawner>()?.KillThemAll();
        }
        if (monsterManager) monsterManager.roomLighting.enabled = false;
        else GetComponent<RoomLighting>().enabled = false;

        Labirint.instance.blueprints[roomID].visited = true;
        Labirint.instance.currentRoomID = -1;
        LockRoom();

        OnAnyRoomLeave.Invoke();
        OnThisLeave.Invoke();
    }

    public void LightsOut()
    {
        if (monsterManager)
        {
            monsterManager.roomLighting.LightsOut();
        }
        else GetComponent<RoomLighting>().LightsOut(); // exception for room without monsters
    }

    public void LightsOn() {
        if (monsterManager != null)
        {
            int monstersToKill = Mathf.Min(monsterManager.EnemyCount(), monsterManager.killsToOpen);
            if (roomType == RoomType.arena && !Labirint.instance.blueprints[roomID].visited)
                monsterManager.roomLighting?.LabirintRoomEnterDark(monstersToKill);
            else
                monsterManager.roomLighting?.LabirintRoomEnterBright();
        }
        else GetComponent<RoomLighting>()?.LabirintRoomEnterBright(); // exception for room without monsters
    }
    
    public Dictionary<Direction.Side, float> GetBordersFromTilemap() {
        Dictionary<Direction.Side, float> result = new Dictionary<Direction.Side, float>();
        Tilemap[] tilemaps = GetComponentsInChildren<Tilemap>();
        float left, right, up, down;
        foreach (Tilemap tilemap in tilemaps)
        {
            if (tilemap.tag == "Environment")
            { // to separete layer with walls
                Vector3Int tilePosition;
                left = Mathf.Infinity;
                right = -Mathf.Infinity;
                up = -Mathf.Infinity;
                down = Mathf.Infinity;
                for (int x = tilemap.origin.x; x < tilemap.size.x; x++)
                {
                    for (int y = tilemap.origin.y; y < tilemap.size.y; y++)
                    {
                        tilePosition = new Vector3Int(x, y, 0);
                        if (tilemap.HasTile(tilePosition))
                        {
                            if (tilemap.CellToWorld(tilePosition).x < left) left = tilemap.CellToWorld(tilePosition).x;
                            if (tilemap.CellToWorld(tilePosition).x > right) right = tilemap.CellToWorld(tilePosition).x;
                            if (tilemap.CellToWorld(tilePosition).y < down) down = tilemap.CellToWorld(tilePosition).y;
                            if (tilemap.CellToWorld(tilePosition).y > up) up = tilemap.CellToWorld(tilePosition).y;
                        }
                    }
                }
                result[Direction.Side.LEFT] = left + 1.5f; //mb need to replace with something tile size related later
                result[Direction.Side.RIGHT] = right + 0.5f;
                result[Direction.Side.UP] = up + 0.5f;
                result[Direction.Side.DOWN] = down + 1.5f;

                break; // ignore stuff like abyss
            }
        }
        return result;
    }

    public bool RectIsInbounds(float x, float y, float sizeX, float sizeY) {
        bool result = true;
        Dictionary<Direction.Side, float> bounds = GetBordersFromTilemap();
        result = x > bounds[Direction.Side.LEFT] &&
            x + sizeX < bounds[Direction.Side.RIGHT] &&
            y > bounds[Direction.Side.DOWN] &&
            y + sizeY < bounds[Direction.Side.UP];
        return result;
    }

    public Tilemap wallsTilemap = null;
    private int brakeCounter = 0;
    private Vector3Int inboundsPosituion;
    public int[,] OOBmap; // 0 - dont know, 1 - oob, 2 - inbounds, 3 - border
    public int topBorder,botBorder,leftBorder,rightBorder;

    public void FillOOB()
    {
        GetWallTilemap();
        Dictionary<Direction.Side, float> borders = GetBordersFromTilemap();
        topBorder = wallsTilemap.WorldToCell(new Vector3(0, borders[Direction.Side.UP], 0)).y + 3;
        botBorder = wallsTilemap.WorldToCell(new Vector3(0, borders[Direction.Side.DOWN], 0)).y - 3;
        leftBorder = wallsTilemap.WorldToCell(new Vector3(borders[Direction.Side.LEFT], 0, 0)).x - 3;
        rightBorder = wallsTilemap.WorldToCell(new Vector3(borders[Direction.Side.RIGHT], 0, 0)).x + 3;

        OOBmap = new int[rightBorder - leftBorder + 1, topBorder - botBorder + 1];
        FillOuterCell2(0, 0);
        brakeCounter = 0;
        if (GetInboundsPoint()) // if can find inside point
        {
            brakeCounter = 0;
            FillInboundsCell2(inboundsPosituion.x-leftBorder, inboundsPosituion.y-botBorder);
            FillRest();
            PaintBorder();
        }
        else {
            OOBmap = null; //to prevent telepot in case of crash
        }
        DrawDebug();
    }

    private void GetWallTilemap()
    { // get walls tilemap layer and set it to var walls
        if (wallsTilemap == null)
        {
            Tilemap[] tilemaps = GetComponentsInChildren<Tilemap>();
            foreach (Tilemap tilemap in tilemaps)
            {
                if (tilemap.tag == "Environment")
                {
                    wallsTilemap = tilemap;
                    break; // ignore stuff like abyss
                }
            }
        }
    }

    //    private void FillOuterCell(int x, int y)
    //    {
    //        OuterDirectionCheck(x, y, x + 1, y);
    //        OuterDirectionCheck(x, y, x - 1, y);
    //        OuterDirectionCheck(x, y, x, y + 1);
    //        OuterDirectionCheck(x, y, x, y - 1);
    //    }    

    //private void OuterDirectionCheck(int oldx, int oldy, int newx, int newy)
    //{
    //    if (newx >= 0 && newy >= 0 &&
    //        newx <= rightBorder-leftBorder && newy <= topBorder-botBorder)
    //    {
    //        if (map[newx, newy] == 0) 
    //        {
    //            if ( !(walls.HasTile(new Vector3Int(oldx + leftBorder, oldy+ botBorder, 0)) && !walls.HasTile(new Vector3Int(newx+ leftBorder, newy+ botBorder, 0))))
    //            {// except transition form filled to not filled tile
    //                map[newx, newy] = 1;
    //                brakeCounter++;
    //                //Debug.Log(newx.ToString() + " " + newy.ToString());
    //                if (brakeCounter < 2000)
    //                {
    //                    FillOuterCell(newx, newy);
    //                }
    //            }
    //        }
    //    }        
    //}

    private void FillOuterCell2(int x, int y) {
        List<Vector3Int> freshCells = new List<Vector3Int>();
        freshCells.Add(new Vector3Int(x, y, 0));
        List<Vector3Int> nextGenegationCells;
        List<Vector3Int> posibleShifts = new List<Vector3Int> { Vector3Int.up, Vector3Int.right, Vector3Int.down, Vector3Int.left };
        Vector3Int arrayToTilemap = new Vector3Int(leftBorder, botBorder, 0);
        Vector3Int newCell;
        while (freshCells.Count > 0) {
            nextGenegationCells = new List<Vector3Int>();
            foreach (Vector3Int oldCell in freshCells) {
                foreach (Vector3Int shift in posibleShifts)
                {
                    brakeCounter++;
                    {
                        newCell = oldCell + shift;
                        if (newCell.x >= 0 && newCell.y >= 0 &&
                        newCell.x <= rightBorder-leftBorder && newCell.y <= topBorder-botBorder)
                        {
                            if ((OOBmap[newCell.x, newCell.y] == 0) && !(wallsTilemap.HasTile(oldCell + arrayToTilemap) && !wallsTilemap.HasTile(newCell + arrayToTilemap)))
                            {
                                nextGenegationCells.Add(newCell);
                                OOBmap[newCell.x, newCell.y] = 1;
                            }
                        }
                    }
                }
            }
            freshCells = new List<Vector3Int>(nextGenegationCells);
        }
    }

    private Vector3 GetRandomDoorPosition() {
        Vector3 result;
        if (doorsSided.ContainsKey(Direction.Side.LEFT)) result = doorsSided[Direction.Side.LEFT].transform.position;
        else if (doorsSided.ContainsKey(Direction.Side.RIGHT)) result = doorsSided[Direction.Side.RIGHT].transform.position;
        else if (doorsSided.ContainsKey(Direction.Side.UP)) result = doorsSided[Direction.Side.UP].transform.position;
        else if (doorsSided.ContainsKey(Direction.Side.DOWN)) result = doorsSided[Direction.Side.DOWN].transform.position;
        else {
            Door door = GetComponentInChildren<Door>();
            if (door != null) result = door.transform.position;
            else result = transform.position; // лучше позиция комнаты, чем ничего если дверей точно нет
        }
        return result;
    }
    private bool GetInboundsPoint() { //эвристика, чертим линию от левой двери вправо пока не найдем пустую клетку.
        bool found = false;
        Vector3Int currentPos = wallsTilemap.WorldToCell(GetRandomDoorPosition());
        while (!found && (currentPos.x<rightBorder)&& brakeCounter<100) {
            //Debug.Log(currentPos.x.ToString()+" "+ rightBorder.ToString());
            brakeCounter++;
            if (!wallsTilemap.HasTile(currentPos)) {
                found = true;
                inboundsPosituion = currentPos;
                return true;
            }
            else
                currentPos += Vector3Int.right;            
        }
    
        if (!found) {
            Debug.Log("Cant find inbounds");
            return false;
        }else return true;
    }

    //private void FillInboundsCell(int x, int y) {
    //    InboundsDirectionCheck(x, y, x + 1, y);
    //    InboundsDirectionCheck(x, y, x - 1, y);
    //    InboundsDirectionCheck(x, y, x, y + 1);
    //    InboundsDirectionCheck(x, y, x, y - 1);
    //}
    //
    //private void InboundsDirectionCheck(int oldx, int oldy, int newx, int newy)
    //{
    //    if (newx >= 0 && newy >= 0 &&
    //        newx <= rightBorder - leftBorder && newy <= topBorder - botBorder)
    //    {
    //        if (map[newx, newy] == 0)
    //        {
    //            if (!walls.HasTile(new Vector3Int(newx + leftBorder, newy + botBorder, 0)))
    //            {
    //                map[newx, newy] = 2;
    //                brakeCounter++;
    //                if (brakeCounter < 3000)
    //                {
    //                    FillInboundsCell(newx, newy);
    //                }
    //            }
    //        }
    //    }
    //}

    private void FillInboundsCell2(int x, int y) {
        List<Vector3Int> freshCells = new List<Vector3Int>();
        freshCells.Add(new Vector3Int(x, y, 0));
        List<Vector3Int> nextGenegationCells;
        Vector3Int[] posibleShifts = new Vector3Int[8] { Vector3Int.up, Vector3Int.up + Vector3Int.right, Vector3Int.right, Vector3Int.right + Vector3Int.down, Vector3Int.down, Vector3Int.down + Vector3Int.left, Vector3Int.left, Vector3Int.left + Vector3Int.up };
        Vector3Int arrayToTilemap = new Vector3Int(leftBorder, botBorder, 0);
        Vector3Int newCell;
        while (freshCells.Count > 0)
        {
            brakeCounter++;
            if (brakeCounter < 10000)
            {
                nextGenegationCells = new List<Vector3Int>();
                foreach (Vector3Int oldCell in freshCells)
                {
                    foreach (Vector3Int shift in posibleShifts)
                    {
                        {
                            newCell = oldCell + shift;
                            if (newCell.x >= 0 && newCell.y >= 0 &&
                            newCell.x <= rightBorder - leftBorder && newCell.y <= topBorder - botBorder)
                            {
                                if ((OOBmap[newCell.x, newCell.y] == 0) && !wallsTilemap.HasTile(newCell + arrayToTilemap))
                                {
                                    nextGenegationCells.Add(newCell);
                                    OOBmap[newCell.x, newCell.y] = 2;
                                }
                            }
                        }
                    }
                }
                freshCells = new List<Vector3Int>(nextGenegationCells);
            }
        }
    }

    private void FillRest() {
        for (int x = 0; x < rightBorder - leftBorder - 1; x++)
        {
            for (int y = 0; y < topBorder - botBorder - 1; y++)
            {
                if (OOBmap[x, y] == 0)
                    OOBmap[x, y] = 1;
            }
        }
    }

    private void PaintBorder() {
        bool isborder;
        for (int x = 1; x < rightBorder - leftBorder - 1; x++)
        {
            for (int y = 1; y < topBorder - botBorder - 1; y++)
            {
                if (OOBmap[x, y] == 1) {
                    isborder = false;
                    isborder = isborder || (OOBmap[x + 1,  y]      == 2); // if has inbounds cell in any of 9 dirrections
                    isborder = isborder || (OOBmap[x + 1,  y + 1]  == 2);
                    isborder = isborder || (OOBmap[x,      y + 1]  == 2);
                    isborder = isborder || (OOBmap[x - 1,  y + 1]  == 2);
                    isborder = isborder || (OOBmap[x - 1,  y]      == 2);
                    isborder = isborder || (OOBmap[x - 1,  y - 1]  == 2);
                    isborder = isborder || (OOBmap[x,      y - 1]  == 2);
                    isborder = isborder || (OOBmap[x + 1,  y - 1]  == 2);
                    if (isborder) OOBmap[x, y] = 3;
                }                
            }
        }
    }

    public bool PositionIsInbounds(Vector3 position) {
        bool result = true;
        if (OOBmap != null && wallsTilemap !=null) {
            Vector3Int positionOnTilemap = wallsTilemap.WorldToCell(position);
            if (positionOnTilemap.x<leftBorder || positionOnTilemap.x>rightBorder ||
                positionOnTilemap.y>topBorder || positionOnTilemap.y<botBorder) {
                result = false;
            }
            else if (OOBmap[positionOnTilemap.x - leftBorder, positionOnTilemap.y - botBorder] == 1)
                result = false;
        }
        return result;
    }

    public bool PositionIsStronglyInbounds(Vector3 position)
    {
        bool result = true;
        if (OOBmap != null && wallsTilemap != null)
        {
            Vector3Int positionOnTilemap = wallsTilemap.WorldToCell(position);
            if (positionOnTilemap.x < leftBorder || positionOnTilemap.x > rightBorder ||
                positionOnTilemap.y > topBorder || positionOnTilemap.y < botBorder)
            {
                result = false;
            }
            else if (OOBmap[positionOnTilemap.x - leftBorder, positionOnTilemap.y - botBorder] != 2)
                result = false;
        }
        return result;
    }

    public Vector3 GetNearInboundsPosition(Vector3 currentPosition) { // осторожно хардкод. Нервным не смотреть
        Vector3 result = currentPosition;
        float shiftAmp = 1;
        Vector3[] possibleShifts = new Vector3[8] { Vector3.up, Vector3.up + Vector3.right, Vector3.right, Vector3.right + Vector3.down, Vector3.down, Vector3.down + Vector3.left, Vector3.left, Vector3.left + Vector3.up };
        for (int i = 1; i < 20; i++)
        {
            shiftAmp = i;
            foreach (Vector3 shift in possibleShifts)
            {
                if (PositionIsStronglyInbounds(currentPosition + (shift * shiftAmp)))
                    return currentPosition + (shift * shiftAmp);
            }
        }
        Debug.Log("Cant find inbounds position");
        return result;
    }

    private void DrawDebug()
    {
        for (int x = leftBorder; x < rightBorder - 1; x++)
            for (int y = botBorder; y < topBorder - 1; y++)
            {
                if (OOBmap[x - leftBorder, y - botBorder] == 1)
                    Debug.DrawRay(wallsTilemap.CellToWorld(new Vector3Int(x, y, 0)), Vector3.up, Color.red, 99f);
                if (OOBmap[x - leftBorder, y - botBorder] == 2)
                    Debug.DrawRay(wallsTilemap.CellToWorld(new Vector3Int(x, y, 0)), Vector3.up, Color.green, 99f);                
                if (OOBmap[x - leftBorder, y - botBorder] == 3)
                    Debug.DrawRay(wallsTilemap.CellToWorld(new Vector3Int(x, y, 0)), Vector3.up, Color.yellow, 99f);
            }
    }
}
