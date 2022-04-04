using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class AllRoadTiles : MonoBehaviour
{
    public static AllRoadTiles ins;

    void Awake()
    {
        ins = this;


        Tilemap tilemap = GetComponent<Tilemap>();
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);
        allTilePos = new Vector2[allTiles.Length];
        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tilemap.GetTile(new Vector3Int(x,y,0)) != null)
                {
                    allTilePos[x + y * bounds.size.x] = new Vector2(x, y);
                    //Debug.Log("x:" + x + " y:" + y + " tile:" + tile.name);
                }
                else
                {
                    //Debug.Log("x:" + x + " y:" + y + " tile: (null)");
                }
            }
        }
        int openPositions = 0;
        for (int i = 0; i < allTilePos.Length; i++)
        {
            if (allTilePos[i] != Vector2.zero)
                openPositions++;
        }
        openTilePos = new Vector2[openPositions];
        int openIndex = 0;
        for (int i = 0; i < allTilePos.Length; i++)
        {
            if (allTilePos[i] != Vector2.zero)
            {
                openTilePos[openIndex] = allTilePos[i];
                openIndex++;
            }
        }
    }
    Vector2[] allTilePos;
    public Vector2[] openTilePos;
    void Start()
    {
    }
}
