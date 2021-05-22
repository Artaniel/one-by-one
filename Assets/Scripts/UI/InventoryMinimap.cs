using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryMinimap : MonoBehaviour
{
    LabirintBuilder labirintBuilder;
    Image[] mapIcons;
    int iconsCount;

    [SerializeField] private Sprite verticalLine = null;
    [SerializeField] private Sprite horizontalLine = null;

    float iconOffsetX = 4.55f;
    float iconOffsetY = -4.25f;
    float iconSizeX = 5.7f;
    float iconSizeY = 5.7f;

    private const int mapSize = 21;
    private Vector2Int center = new Vector2Int(10, 10);

    void Start()
    {
        if (!Labirint.instance.OneRoomMode)
        {
            mapIcons = GetComponentsInChildren<Image>();
            foreach (var icon in mapIcons)
            {
                icon.enabled = false;
            }
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

        int j = 0;

        for (int i = 0; i < allRoomPositions.Count; i++)
        {
            var position = ToMapSpace(currentRoomPosition, allRoomPositions[i]);
            if (ValidPosition(position))
            {
                mapIcons[i].enabled = true;
                mapIcons[i].rectTransform.anchoredPosition = new Vector2(iconOffsetX + iconSizeX * position.x, -(iconOffsetY + iconSizeY * position.y));
            }
            //print($"{i} {allRoomPositions[i]} {position}");

            foreach (Direction.Side side in Direction.sides)
            {
                if (roomBlueprints[i].rooms.ContainsKey(side))
                {
                    if (roomBlueprints[i].rooms[side] != -1)
                    {
                        var neighborPosition = ToMapSpace(currentRoomPosition, allRoomPositions[roomBlueprints[i].rooms[side]]);
                        if (!ValidPosition(neighborPosition)) continue;
                        var difference = position - neighborPosition;
                        var halfDifference = difference;
                        halfDifference.x /= 2;
                        halfDifference.y /= 2;
                        Vector2Int linePosition = position - halfDifference;
                        if (difference.x == 2 || difference.x == -2)
                        {
                            mapIcons[allRoomPositions.Count + j].sprite = horizontalLine;
                        }
                        else
                        {
                            mapIcons[allRoomPositions.Count + j].sprite = verticalLine;
                        }
                        mapIcons[allRoomPositions.Count + j].enabled = true;
                        mapIcons[allRoomPositions.Count + j].rectTransform.anchoredPosition = 
                            new Vector2(iconOffsetX + iconSizeX * linePosition.x, -(iconOffsetY + iconSizeY * linePosition.y));
                        j++;
                    }
                }
            }
        }
    }

    Vector2Int ToMapSpace(Vector2Int currentPosition, Vector2Int coordinates) // 60 is a center, 11 elements in rows and columns 
    {
        Vector2Int newPosition = currentPosition; // because UI map is Y-inverted
        newPosition.x = coordinates.x - currentPosition.x;
        newPosition.y = currentPosition.y - coordinates.y;
        return center + (newPosition * 2);
    }

    bool ValidPosition(Vector2Int position) => position.x >= 0 && position.y >= 0 && position.x < mapSize && position.y < mapSize;
}
