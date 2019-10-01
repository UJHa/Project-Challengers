using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager gameInstance;

    public Tilemap tilemap;
    private int[,] array2D = new int[8, 8];

    private Sprite[] sprites;
    public GameObject character;
    public Vector3Int charPosition;
    // Start is called before the first frame update
    private void Awake()
    {
        gameInstance = this;
    }

    void Start()
    {
        Debug.Log("Start GameManager!!!");

        sprites = Resources.LoadAll<Sprite>("0x72_16x16DungeonTileset.v4");
        Debug.Log("sprites : "+sprites.Length);
        //cube.transform.position = new Vector3(x, y, 0);
        GameObject player = Instantiate(character);

        //타일 데이터 관련 진행 예정
        //for (int i = 0; i < 8; i++)
        //{
        //    for (int j = 0; j < 8; j++)
        //    {
        //        array2D[i, j] = 0;
        //    }
        //}
        //
        //array2D[3, 4] = 1;

        player.transform.position = tilemap.layoutGrid.CellToWorld(charPosition);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
