using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    public static GameManager gameInstance;

    public Tilemap tilemap;

    public Button buySlot1;
    public Button buySlot2;
    public Button buySlot3;
    public Button buySlot4;

    public Button resetBtn;

    public enum eCharacter
    {
        BLOBMINION,
        CYCLOPS,
        DETECTIVE,
        DWARF,
        IMP,
        KNIGHT,
        LIZARD,
        PLASMADRONE,
        ROYALKNIGHT,
        SANTA,
        SKELETON,
        SAPCECADET,
        TAURUS,
        VEX,
        MAX
    }

    // Start is called before the first frame update
    private void Awake()
    {
        gameInstance = this;
    }

    void Start()
    {
        Debug.Log("Start GameManager!!!");
        //타일 객체들 생성(타일 위 오브젝트 관리를 위해서...)
        for (int i = -1; i < 8; i++)    //-1은 체스말 대기 위치
        {
            //tileCharacters.Add(new List<ChessTile>());
            for (int j = 0; j < 8; j++)
            {
                ChessTile chessTile = ScriptableObject.CreateInstance<ChessTile>();
                if (tilemap.GetTile(new Vector3Int(j, i, 0)))
                {
                    chessTile.sprite = Resources.Load<Sprite>("Blocks/" + tilemap.GetTile(new Vector3Int(j, i, 0)).name);
                }
                chessTile.colliderType = Tile.ColliderType.None;
                chessTile.SetTilePosition(new Vector3Int(j, i, 0));
                tilemap.SetTile(chessTile.GetTilePosition(), chessTile);
            }
        }

        //캐릭터 구매 UI 초기화
        {
            SetBuySlot(buySlot1, GetRandomCharacter());
            SetBuySlot(buySlot2, GetRandomCharacter());
            SetBuySlot(buySlot3, GetRandomCharacter());
            SetBuySlot(buySlot4, GetRandomCharacter());
        }
        resetBtn.onClick.AddListener(() => {
            {
                SetBuySlot(buySlot1, GetRandomCharacter());
                SetBuySlot(buySlot2, GetRandomCharacter());
                SetBuySlot(buySlot3, GetRandomCharacter());
                SetBuySlot(buySlot4, GetRandomCharacter());
            }
        });

        //캐릭 생성 관련 테스트
        {
            //SpawnCharacter("Prefabs/Lizard", "Player", 0, 3, true);   //player
            //SpawnCharacter("Prefabs/Knight", "Knight(NPC)", 1, 3, false);
            //SpawnCharacter("Prefabs/Lizard", "Lizard(NPC)", 2, 3, false);
            //SpawnCharacter("Prefabs/Skeleton", "Skeleton(NPC)", 3, 3, false);
            //SpawnCharacter("Prefabs/Knight", 3, 3, false);
        }
    }

    private GameObject holdTarget;
    private ChessTile mouseDownTile;
    // Update is called once per frame
    void Update()
    {
        if (tilemap != null)
        {
            tilemap.RefreshAllTiles();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3Int mouseDownTilePosition = tilemap.layoutGrid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            //mouseTargetTilePosition의 x,y가 -1씩 되어 있다 원인은 파악하지 못해서 임시 처리
            mouseDownTilePosition.x += 1;
            mouseDownTilePosition.y += 1;
            mouseDownTilePosition.z = 0;
            mouseDownTile = tilemap.GetTile<ChessTile>(mouseDownTilePosition);
            if (mouseDownTile != null)
            {
                if (mouseDownTile.gameObject != null)
                {
                    holdTarget = mouseDownTile.gameObject;
                    mouseDownTile.gameObject = null;
                }
            }
        }

        //hold 도중 이동처리
        if (Input.GetMouseButton(0) && holdTarget != null)
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.x += 0;
            mouseWorldPosition.y += 0.5f;
            mouseWorldPosition.z = 0;
            holdTarget.transform.position = mouseWorldPosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (holdTarget != null)
            {
                Debug.Log("holdTarget null 아닐때");
                Vector3Int mouseUpTilePosition = tilemap.layoutGrid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));

                //mouseTargetTilePosition의 x,y가 -1씩 되어 있다 원인은 파악하지 못해서 임시 처리
                mouseUpTilePosition.x += 1;
                mouseUpTilePosition.y += 1;
                mouseUpTilePosition.z = 0;

                ChessTile mouseUpTile = tilemap.GetTile<ChessTile>(mouseUpTilePosition);
                if (mouseUpTile != null)
                {
                    if (mouseUpTile.GetTilePosition().y >= 0 && mouseUpTile.GetTilePosition().y < 4) // 4x8 배치 되는 기능 구현
                    {
                        Debug.Log("y : 0 이상");
                        string[] nameList = holdTarget.name.Split('_');
                        SpawnCharacter("Prefabs/"+ nameList[1], nameList[1] + "(NPC)", mouseUpTile.GetTilePosition().x, mouseUpTile.GetTilePosition().y, false);
                        //Debug.Log("=start====================");
                        //AllTilesLog();
                        //Debug.Log("=end======================");
                        Destroy(holdTarget);
                        //mouseDownTile.gameObject = holdTarget;
                        //mouseDownTile.gameObject.transform.position = tilemap.layoutGrid.CellToWorld(mouseDownTile.position);
                    }
                    else if(mouseUpTile.GetTilePosition().y == -1)    //대기 타일 내 이동
                    {
                        Debug.Log("y : -1");
                        if (mouseUpTile.gameObject == null) // 빈 타일일 때
                        {
                            mouseUpTile.gameObject = holdTarget;
                            mouseUpTile.gameObject.transform.position = tilemap.layoutGrid.CellToWorld(mouseUpTile.GetTilePosition());
                        }
                        else    //타일 있을 때 서로의 위치 교체하기
                        {
                            GameObject tempObject = mouseUpTile.gameObject;

                            mouseUpTile.gameObject = holdTarget;
                            mouseUpTile.gameObject.transform.position = tilemap.layoutGrid.CellToWorld(mouseUpTile.GetTilePosition());

                            mouseDownTile.gameObject = tempObject;
                            mouseDownTile.gameObject.transform.position = tilemap.layoutGrid.CellToWorld(mouseDownTile.GetTilePosition());
                        }
                    }
                    else    //이상한 곳에 이동시키면... 실패시키기
                    {
                        Debug.Log("y : 예외들");
                        mouseDownTile.gameObject = holdTarget;
                        mouseDownTile.gameObject.transform.position = tilemap.layoutGrid.CellToWorld(mouseDownTile.GetTilePosition());
                    }
                    
                    for (int i = 0; i < 8; i++)
                    {
                        ChessTile waitTile = tilemap.GetTile<ChessTile>(new Vector3Int(i, -1, 0));
                        if (waitTile != null)
                        {
                            //Debug.Log("waitTile[" + i + "] : " + waitTile.gameObject);
                        }
                    }
                }
                else
                {
                    Debug.Log("y : 예외들2");
                    mouseDownTile.gameObject = holdTarget;
                    mouseDownTile.gameObject.transform.position = tilemap.layoutGrid.CellToWorld(mouseDownTile.GetTilePosition());
                }

                holdTarget = null;
            }
        }
    }

    private string GetRandomCharacter()
    {
        string name = "";
        eCharacter enumCharacter = (eCharacter)Random.Range(0, (int)eCharacter.MAX);
        switch (enumCharacter)
        {
            case eCharacter.BLOBMINION:
                name = "BlobMinion";
                break;
            case eCharacter.CYCLOPS:
                name = "Cyclops";
                break;
            case eCharacter.DETECTIVE:
                name = "Detective";
                break;
            case eCharacter.DWARF:
                name = "Dwarf";
                break;
            case eCharacter.IMP:
                name = "Imp";
                break;
            case eCharacter.KNIGHT:
                name = "Knight";
                break;
            case eCharacter.LIZARD:
                name = "Lizard";
                break;
            case eCharacter.PLASMADRONE:
                name = "PlasmaDrone";
                break;
            case eCharacter.ROYALKNIGHT:
                name = "RoyalKnight";
                break;
            case eCharacter.SANTA:
                name = "Santa";
                break;
            case eCharacter.SKELETON:
                name = "Skeleton";
                break;
            case eCharacter.TAURUS:
                name = "Taurus";
                break;
            case eCharacter.SAPCECADET:
                name = "SpaceCadet";
                break;
            case eCharacter.VEX:
                name = "Vex";
                break;
        }
        return name;
    }

    private void SetBuySlot(Button buySlot, string name)
    {
        buySlot.gameObject.SetActive(true);
        //이미지 세팅
        GameObject imageObject = buySlot.gameObject.transform.Find("Image").gameObject;
        Image image = imageObject.GetComponent<Image>();
        image.sprite = Resources.Load<Sprite>("BuySlotImages/" + name);
        ResizeImage(image, imageObject);

        //이름 세팅
        GameObject nameTextObject = buySlot.gameObject.transform.Find("Text").gameObject;
        Text nameText = nameTextObject.GetComponent<Text>();
        nameText.text = name;

        buySlot.onClick.RemoveAllListeners();
        buySlot.onClick.AddListener(() => {
            bool success = BuyWaitCharacter("Prefabs/Wait" + name, "WaitCharacter_" + name);
            if (success)
            {
                buySlot.gameObject.SetActive(false);
            }
        });
    }
    private void ResizeImage(Image image, GameObject imageObject)
    {
        RectTransform imageRect = imageObject.GetComponent<RectTransform>();
        imageRect.sizeDelta = new Vector2(75, 75);
        float rateWidth = imageRect.rect.width * (image.sprite.rect.width / image.sprite.rect.height);
        imageRect.sizeDelta = new Vector2(rateWidth, imageRect.rect.height);
    }

    private bool BuyWaitCharacter(string path, string name)
    {
        int waitPositionX = 0;
        while (waitPositionX < 8)
        {
            ChessTile chessTile = tilemap.GetTile<ChessTile>(new Vector3Int(waitPositionX, -1, 0));
            Debug.Log("chessTile.gameObject["+waitPositionX+"] : " + chessTile.gameObject);
            if (chessTile != null)
            {
                if (chessTile.gameObject == null)
                {
                    GameObject character = Instantiate(Resources.Load(path)) as GameObject;
                    character.name = name;
                    ChessCharacter cCharacter = character.GetComponent<ChessWaitCharacter>();
                    character.transform.SetParent(tilemap.transform);

                    cCharacter.SetTilePosition(new Vector3Int(waitPositionX, -1, 0));
                    chessTile.gameObject = character;
                    return true;
                }
            }
            waitPositionX++;
        }
        return false;
    }

    private void SpawnCharacter(string path, string name, int tileX, int tileY, bool isPlayer)
    {
        GameObject character = Instantiate(Resources.Load(path)) as GameObject;
        character.name = name;
        ChessCharacter cCharacter = character.GetComponent<ChessCharacter>();
        cCharacter.SetTilePosition(new Vector3Int(tileX, tileY, 0));
        cCharacter.IsPlayer(isPlayer);
        character.transform.SetParent(tilemap.transform);

        ChessTile chessTile = tilemap.GetTile<ChessTile>(new Vector3Int(tileX, tileY, 0));
        if (chessTile != null)
        {
            chessTile.gameObject = character;
        }

        GameObject canvas = GameObject.Find("Canvas");
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        //slider transform 세팅
        GameObject sliderObject = Instantiate(Resources.Load("Prefabs/HpBar")) as GameObject;
        sliderObject.transform.SetParent(canvas.transform, false);

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
        ChessTile chessTile = tilemap.GetTile<ChessTile>(tilePosition);
        if (chessTile != null)
        {
            chessTile.gameObject = gameObject;
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

        ChessTile chessTile = tilemap.GetTile<ChessTile>(tilePosition);
        if (chessTile != null)
        {
            gameObject = chessTile.gameObject;
        }
        return gameObject;
    }

    public void ResetTilePath(string keyName)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                ChessTile chessTile = tilemap.GetTile<ChessTile>(new Vector3Int(j, i, 0));
                chessTile.ResetTileData(keyName);
            }
        }
    }

    public void AllTilesLog()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                ChessTile chessTile = tilemap.GetTile<ChessTile>(new Vector3Int(j, i, 0));
                if (chessTile != null)
                {
                    Debug.Log("tile["+j+","+i+"] : " + tilemap.GetColliderType(chessTile.GetTilePosition()));
                }
            }
        }
    }
}