using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryMinimap : MonoBehaviour
{
    LabirintBuilder labirintBuilder;
    Image[] mapIcons;
    int iconsCount;

    [SerializeField] private Sprite verticalLine;
    [SerializeField] private Sprite horizontalLine;

    float iconOffsetX = 4.55f;
    float iconOffsetY = -4.25f;
    float iconSizeX = 5.7f;
    float iconSizeY = 5.7f;

    void Start()
    {
        if (!Labirint.instance.OneRoomMode)
        {
            mapIcons = GetComponentsInChildren<Image>();
            iconsCount = mapIcons.Length;
            labirintBuilder = Labirint.instance.GetComponent<LabirintBuilder>();
            Room.OnAnyRoomEnter.AddListener(UpdateMap);
            UpdateMap();
        }
    }

    void UpdateMap()
    {
        var allRoomPositions = labirintBuilder.allRoomsPositions;
        var roomBlueprints = Labirint.instance.blueprints;
        int currentRoomID = Labirint.instance.currentRoomID;
        Vector2Int currentRoomPosition = allRoomPositions[currentRoomID];

        for (int i = 0; i < iconsCount; i++)
        {
            mapIcons[i].color = Color.clear;
        }

        print(roomBlueprints.Length);
        print(allRoomPositions.Count);

        for (int i = 0; i < allRoomPositions.Count; i++)
        {
            var position = ToMapSpace(currentRoomPosition, allRoomPositions[i]);
            if (position >= 0 && position < iconsCount)
            {
                mapIcons[position].color = Color.white;
            }
            //print($"{i} {allRoomPositions[i]} {position}");

            foreach (Direction.Side side in Direction.sides)
            {
                if (roomBlueprints[i].rooms.ContainsKey(side))
                {
                    if (roomBlueprints[i].rooms[side] != -1)
                    {
                        var neighborPosition = ToMapSpace(currentRoomPosition, allRoomPositions[roomBlueprints[i].rooms[side]]);
                        var difference = position - neighborPosition;
                        if (difference == 2 || difference == -2)
                        {
                            int linePosition = position - difference / 2;
                            if (linePosition >= 0 && linePosition < iconsCount)
                            {
                                mapIcons[linePosition].sprite = horizontalLine;
                                mapIcons[linePosition].color = Color.white;
                            }
                        }
                        else
                        {
                            int linePosition = position - difference / 2;
                            if (linePosition >= 0 && linePosition < iconsCount)
                            {
                                mapIcons[linePosition].sprite = verticalLine;
                                mapIcons[linePosition].color = Color.white;
                            }
                        }
                    }
                }
            }
        }
    }

    int ToMapSpace(Vector2Int currentPosition, Vector2Int coordinates) // 60 is a center, 11 elements in rows and columns 
    {
        int xDiff = coordinates.x - currentPosition.x;
        if (Mathf.Abs(xDiff) > 21) return 999999;
        return iconsCount / 2 + ((currentPosition.y - coordinates.y) * 42) + ((coordinates.x - currentPosition.x) * 2);
    }
}
