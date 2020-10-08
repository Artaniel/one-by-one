using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ShadowPlacer : MonoBehaviour
{
    [SerializeField, Tooltip("Tilemap to place shadow under")]
    private Tilemap referenceTilemap = null;
    [SerializeField]
    private Tile shadowTile = null;
    [SerializeField]
    private float alphaValue = 1f;

    public void PlaceShadows()
    {
        var shadowTilemapObj = new GameObject("Shadow Tilemap");
        shadowTilemapObj.AddComponent<Tilemap>();
        var renderer = shadowTilemapObj.AddComponent<TilemapRenderer>();
        renderer.sortingOrder = -4;

        var tilemap = referenceTilemap;
        Vector3Int tilePosition;
        for (int x = tilemap.origin.x; x < tilemap.size.x; x++)
        {
            for (int y = tilemap.origin.y; y < tilemap.size.y; y++)
            {
                tilePosition = new Vector3Int(x, y, 0);
                if (tilemap.HasTile(tilePosition))
                {
                    //destinationTilemap.SetTile(tilePosition, shadowTile);
                }
            }
        }
    }
}
