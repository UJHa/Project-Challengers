using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager gameInstance;

    public Button upBtn;
    public Button downBtn;
    public Button leftBtn;
    public Button rightBtn;

    public Tilemap tilemap;
    private int[,] tileDatas = new int[8, 8];

    //public GameObject character;
    //public Vector3Int charPosition;
    // Start is called before the first frame update
    private void Awake()
    {
        gameInstance = this;
    }

    void Start()
    {
        Debug.Log("Start GameManager!!!");

		SpawnCharacter("Prefabs/Knight", 7, 3, true);   //player
        SpawnCharacter("Prefabs/Knight", 1, 3, false);
        SpawnCharacter("Prefabs/Knight", 3, 3, false);

        Debug.Log("tilemap : " + tilemap.GetTile(new Vector3Int(0, 0, 0)));
        
        //타일 데이터 리셋
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                tileDatas[i, j] = 0;
                tilemap.SetColliderType(new Vector3Int(i, j, 0), Tile.ColliderType.None);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void SpawnCharacter(string path, int tileX, int tileY, bool isPlayer)
	{
		GameObject character = Instantiate(Resources.Load(path)) as GameObject;
		Knight knight = character.GetComponent<Knight>();
        knight.SetTilePosition(new Vector3Int(tileX, tileY, 0));
		knight.IsPlayer(isPlayer);

        if (isPlayer)
        {
            upBtn.onClick.AddListener(() => { knight.MoveRequest(Knight.Direction.UP); });
            downBtn.onClick.AddListener(() => { knight.MoveRequest(Knight.Direction.DOWN); });
            leftBtn.onClick.AddListener(() => { knight.MoveRequest(Knight.Direction.LEFT); });
            rightBtn.onClick.AddListener(() => { knight.MoveRequest(Knight.Direction.RIGHT); });
        }
    }

    public void SetTileData(Vector3Int tilePos, int data)
    {
        tileDatas[tilePos.x, tilePos.y] = data;
        tilemap.SetColliderType(new Vector3Int(tilePos.x, tilePos.y, 0), (Tile.ColliderType)data);
    }

    public int GetTileData(Vector3Int tilePos)
    {
        return tileDatas[tilePos.x, tilePos.y];
    }

    public void SendMessageForTile(Vector3Int tilePosition)
    {

    }
}