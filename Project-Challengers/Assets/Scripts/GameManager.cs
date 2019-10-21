using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    public static GameManager gameInstance;

    public Button upBtn;
    public Button downBtn;
    public Button leftBtn;
    public Button rightBtn;

    public Tilemap tilemap;

    private List<List<ChessTile>> tileCharacters = new List<List<ChessTile>>();

    // Start is called before the first frame update
    private void Awake()
    {
        gameInstance = this;
    }

    void Start()
    {
        Debug.Log("Start GameManager!!!");

        //타일 데이터 리셋
        //Tile tile = ScriptableObject.CreateInstance<Tile>();
        //tile.sprite = Resources.Load<Sprite>("Blocks/Winter 1");
        //tile.colliderType = Tile.ColliderType.None;

        //바닥 타일 생성
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                //tilemap.SetTile(new Vector3Int(i, j, 0), tile);
                tilemap.SetColliderType(new Vector3Int(i, j, 0), Tile.ColliderType.None);
            }
        }

        //타일 객체들 생성(타일 위 오브젝트 관리를 위해서...)
        for (int i = 0; i < 8; i++)
        {
            tileCharacters.Add(new List<ChessTile>());
            for (int j = 0; j < 8; j++)
            {
                tileCharacters[i].Add(new ChessTile());
                //Debug.Log("tileCharacters[i][j];" + tileCharacters[i][j]);
            }
        }

        //타일 생성 테스트
        //tile.sprite = Resources.Load<Sprite>("Blocks/Autumn 1");
        //tile.colliderType = Tile.ColliderType.None;
        //tilemap.SetTile(new Vector3Int(7, 0, 1), tile);
        //Debug.Log(tilemap.GetTile(new Vector3Int(7, 0, 1)));

        SpawnCharacter("Prefabs/Skeleton", "Player", 0, 3, true);   //player
        SpawnCharacter("Prefabs/Skeleton", "Knight(NPC)", 1, 3, false);
        SpawnCharacter("Prefabs/Skeleton", "Skeleton(NPC)", 2, 3, false);
        SpawnCharacter("Prefabs/Skeleton", "Knight(NPC)", 3, 3, false);
        //SpawnCharacter("Prefabs/Knight", 3, 3, false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void SpawnCharacter(string path, string name, int tileX, int tileY, bool isPlayer)
    {
        GameObject character = Instantiate(Resources.Load(path)) as GameObject;
        character.name = name;
        ChessCharacter cCharacter = character.GetComponent<ChessCharacter>();
        //Debug.Log("Spawn : " + cCharacter);
        cCharacter.SetTilePosition(new Vector3Int(tileX, tileY, 0));
        cCharacter.IsPlayer(isPlayer);

        tileCharacters[tileY][tileX].SetGameObject(character);

        if (isPlayer)
        {
            upBtn.onClick.AddListener(() => { cCharacter.MoveRequest(ChessCharacter.Direction.UP); });
            downBtn.onClick.AddListener(() => { cCharacter.MoveRequest(ChessCharacter.Direction.DOWN); });
            leftBtn.onClick.AddListener(() => { cCharacter.MoveRequest(ChessCharacter.Direction.LEFT); });
            rightBtn.onClick.AddListener(() => { cCharacter.MoveRequest(ChessCharacter.Direction.RIGHT); });
        }

        GameObject canvas = GameObject.Find("Canvas");
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        //slider transform 세팅
        GameObject sliderObject = Instantiate(Resources.Load("Prefabs/HpBar")) as GameObject;
        sliderObject.transform.parent = canvas.transform;
        sliderObject.transform.localScale = new Vector3(1,1,1);

        //Vector3 characterPos = tilemap.layoutGrid.CellToWorld(cCharacter.GetTilePosition());
        //Vector3 characterUiPos = Camera.main.WorldToScreenPoint(characterPos) + new Vector3(canvasRect.rect.x, canvasRect.rect.y, 0);
        //sliderObject.transform.localPosition = characterUiPos;
        Slider slider = sliderObject.GetComponent<Slider>();
        cCharacter.SetHpBar(slider);
    }

    public void SetTileObject(Vector3Int tilePosition, GameObject gameObject)
    {
        if (tilePosition.x < 0 || tilePosition.x > 7
         || tilePosition.y < 0 || tilePosition.y > 7)
        {
            Debug.Log("Tile 범위 초과 : GetTileObject");
        }
        ChessTile chessTile = tileCharacters[tilePosition.y][tilePosition.x];
        if (chessTile != null)
        {
            chessTile.SetGameObject(gameObject);
        }
    }

    public GameObject GetTileObject(Vector3Int tilePosition)
    {
        GameObject gameObject = null;
        if (tilePosition.x < 0 || tilePosition.x > 7
         || tilePosition.y < 0 || tilePosition.y > 7)
        {
            return null;
        }
        ChessTile chessTile = tileCharacters[tilePosition.y][tilePosition.x];
        if (chessTile != null)
        {
            gameObject = chessTile.GetGameObject();
        }
        return gameObject;
    }
}