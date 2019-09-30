using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
	public Tilemap tilemap;

    private Sprite[] sprites;
    public GameObject character;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start GameManager!!!");

        sprites = Resources.LoadAll<Sprite>("0x72_16x16DungeonTileset.v4");
        Debug.Log("sprites : "+sprites.Length);
        //cube.transform.position = new Vector3(x, y, 0);
        GameObject player = Instantiate(character);

		Debug.Log("bounds START");
		BoundsInt bounds = tilemap.cellBounds;
		Debug.Log("bounds : " + bounds);
		Debug.Log("bounds.min : " + bounds.min);
		Debug.Log("bounds.max : " + bounds.max);
		Debug.Log("cell world bounds.min : " + tilemap.CellToWorld(bounds.min));
		Debug.Log("cell world bounds.max : " + tilemap.CellToWorld(bounds.max));
		Debug.Log("tilemap.origin.x : " + tilemap.origin.x);
		Debug.Log("tilemap.origin.y : " + tilemap.origin.y);
		TileBase[] allTiles = tilemap.GetTilesBlock(bounds);
		TileBase tile = tilemap.GetTile(new Vector3Int(0, 0, 0));
		Debug.Log("name : " + tilemap.GetUsedTilesCount());
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
