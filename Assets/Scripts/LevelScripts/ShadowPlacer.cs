using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class ShadowPlacer : MonoBehaviour
{
    [SerializeField, Tooltip("Tilemap to place shadow under"), Header("This component should be placed on Grid that contains tilemaps")]
    private Tilemap referenceTilemap = null;
    [SerializeField, Tooltip("Floor tilemap")]
    private Tilemap floorTilemap = null;
    [SerializeField]
    private TileBase shadowTile = null;
    [SerializeField]
    private float alphaValue = 0.35f;

    public void Awake()
    {
        floorTilemap = transform.GetChild(0).GetComponent<Tilemap>();
        referenceTilemap = transform.GetChild(1).GetComponent<Tilemap>();
    }

    public void PlaceShadows()
    {
        var shadowTilemapObj = new GameObject("Shadow Tilemap");
        var destinationTilemap = shadowTilemapObj.AddComponent<Tilemap>();
        var color = destinationTilemap.color;
        color.a = alphaValue;
        destinationTilemap.color = color;

        var renderer = shadowTilemapObj.AddComponent<TilemapRenderer>();
        renderer.sortingOrder = -4;
        
        Vector3Int tilePosition, floorPosition;
        for (int x = referenceTilemap.origin.x; x < referenceTilemap.size.x; x++)
        {
            for (int y = referenceTilemap.origin.y; y < referenceTilemap.size.y; y++)
            {
                tilePosition = new Vector3Int(x, y, 0);
                if (referenceTilemap.HasTile(tilePosition))
                {
                    for (int hor = -1; hor <= 1; hor++)
                    {
                        for (int vert = -1; vert <= 1; vert++)
                        {
                            floorPosition = new Vector3Int(x + hor, y + vert, 0);
                            if (floorTilemap.HasTile(floorPosition))
                            {
                                destinationTilemap.SetTile(tilePosition, shadowTile);
                                hor += 10; vert += 10; // break cycle
                            }
                        }
                    }
                }
            }
        }

        shadowTilemapObj.transform.position = referenceTilemap.transform.position;
        shadowTilemapObj.transform.localScale = referenceTilemap.transform.localScale * transform.localScale.x;
        shadowTilemapObj.transform.SetParent(transform);
    }
}
