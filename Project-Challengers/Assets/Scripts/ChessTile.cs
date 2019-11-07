using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChessTile : Tile
{
    private Dictionary<string, ChessTile> prevPathTileNodeMap;
    private Vector3Int _tilePosition;
    private int distanceWeight;

    public ChessTile()
    {
        prevPathTileNodeMap = new Dictionary<string, ChessTile>();
    }

    public void ResetTileData(string keyName)
    {
        prevPathTileNodeMap[keyName] = null;
        distanceWeight = 0;
    }

    public void SetPrevPathTileNodeMap(string keyName, ChessTile tile)
    {
        prevPathTileNodeMap[keyName] = tile;
    }

    public ChessTile GetPrevPathTileNodeMap(string keyName)
    {
        return prevPathTileNodeMap[keyName];
    }

    public void SetTilePosition(Vector3Int tilePosition)
    {
        _tilePosition = tilePosition;
    }

    public Vector3Int GetTilePosition()
    {
        return _tilePosition;
    }
}
