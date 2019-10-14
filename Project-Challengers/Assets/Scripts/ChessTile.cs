using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChessTile
{
    private GameObject _gameObject;

    public void SetGameObject(GameObject gameObject)
    {
        _gameObject = gameObject;
    }

    public GameObject GetGameObject()
    {
        return _gameObject;
    }
}
