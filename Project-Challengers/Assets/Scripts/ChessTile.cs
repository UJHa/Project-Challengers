using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChessTile : Tile
{
    public Dictionary<string, ChessTile> prevPathTileNodeMap;
    public Vector3Int position;

    public ChessTile()
    {
        prevPathTileNodeMap = new Dictionary<string, ChessTile>();
    }
}
