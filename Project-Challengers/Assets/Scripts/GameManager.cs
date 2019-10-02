using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager gameInstance;

    public Tilemap tilemap;
    private int[,] array2D = new int[8, 8];

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

		SpawnCharacter("Prefabs/Knight", 6, 3, true);
		SpawnCharacter("Prefabs/Knight", 5, 3, false);

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
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	private void SpawnCharacter(string path, int tileX, int tileY, bool isPlayer)
	{
		GameObject player = Instantiate(Resources.Load(path)) as GameObject;
		Knight knight = player.GetComponent<Knight>();
		knight.tileX = tileX;
		knight.tileY = tileY;
		knight.IsPlayer(isPlayer);
	}
}
