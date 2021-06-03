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
    [SerializeField] private Image minimapImageForNoLabirint = null;

    float iconOffsetX = 4.55f;
    float iconOffsetY = -4.25f;
    float iconSizeX = 5.7f;
    float iconSizeY = 5.7f;

    float iconSizeCurrentRoomX = 8.5f;
    float iconSizeCurrentRoomY = 8.5f;

    [SerializeField] private Color currentRoomColor = Color.white;
    [SerializeField] private Color visitedRoomColor = new Color32(255, 238, 224, 255);
    [SerializeField] private Color treasureRoomColor = new Color32(253, 251, 171, 255);
    [SerializeField] private Color exitColor = new Color32(142, 77, 77, 255);
    [SerializeField] private Color32 nonVisitedColor = new Color32(173, 141, 114, 255);
    [SerializeField] private Color32 startRoomColor = new Color32(255, 117, 117, 255);
    [SerializeField] private Color32 nonVisitedTransitionColor = new Color32(138, 145, 171, 255);
    [SerializeField] private Color32 visitedTransitionColor = new Color32(255, 255, 255, 255);


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
            minimapImageForNoLabirint.enabled = false; 
        }
    }

    void UpdateMap()
    {
        var allRoomPositions = labirintBuilder.allRoomsPositions;
        var labirint = Labirint.instance;
        var roomBlueprints = labirint.blueprints;
        int currentRoomID = labirint.currentRoomID;
        Vector2Int currentRoomPosition = allRoomPositions[currentRoomID];

        // this cycle draws lines between rooms
        // lines should be drawn beneath the icons. That is why we do it in two cycles
        int j = 0;
        for (int i = 0; i < allRoomPositions.Count; i++)
        {
            var position = ToMapSpace(currentRoomPosition, allRoomPositions[i]);
            if (!ValidPosition(position)) continue;

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
                        var icon = mapIcons[j];
                        if (difference.x == 2 || difference.x == -2)
                        {
                            icon.sprite = horizontalLine;
                        }
                        else
                        {
                            icon.sprite = verticalLine;
                        }
                        icon.enabled = true;
                        icon.rectTransform.anchoredPosition =
                            new Vector2(iconOffsetX + iconSizeX * linePosition.x, -(iconOffsetY + iconSizeY * linePosition.y));
                        icon.color = roomBlueprints[i].visited || i == currentRoomID ? visitedTransitionColor : nonVisitedTransitionColor;

                        j++;
                    }
                }
            }
        }

        // this cycle draws rooms on minimap
        for (int i = 0; i < allRoomPositions.Count; i++)
        {
            var position = ToMapSpace(currentRoomPosition, allRoomPositions[i]);
            if (ValidPosition(position))
            {
                var icon = mapIcons[j + i];
                icon.enabled = true;

                icon.rectTransform.sizeDelta = new Vector2(iconSizeX, iconSizeY);
                icon.rectTransform.anchoredPosition = new Vector2(iconOffsetX + iconSizeX * position.x, -(iconOffsetY + iconSizeY * position.y));
                if (roomBlueprints[i].contanerPrefab != null) // treasure room
                {
                    icon.color = treasureRoomColor;
                }
                else if (roomBlueprints[i].exitSceneName != "")
                {
                    icon.color = exitColor;
                }
                else if (i == 0)
                {
                    icon.color = startRoomColor;
                }
                else if (roomBlueprints[i].visited)
                {
                    icon.color = visitedRoomColor;
                }
                else
                {
                    icon.color = nonVisitedColor;
                }
                    


                if (currentRoomPosition == allRoomPositions[i])
                {
                    icon.rectTransform.sizeDelta = new Vector2(iconSizeCurrentRoomX, iconSizeCurrentRoomY);
                    icon.rectTransform.anchoredPosition = 
                        new Vector2(
                            iconOffsetX + ((iconSizeX - iconSizeCurrentRoomX) / 2f) + iconSizeX * position.x,
                            -(iconOffsetY + ((iconSizeY - iconSizeCurrentRoomY) / 2f) + iconSizeY * position.y)); // it just works
                    icon.color = currentRoomColor;
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
