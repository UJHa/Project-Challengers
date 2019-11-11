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

    public Text roundNum;
    public Text roundTimer;
    public Text roundInfo;
    public int currentRound = 0;
    public int maxRound = 7;
    private Dictionary<int, List<eCharacter>> roundEnemyList;
    public List<string> lastPlayerNameList;
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

    public enum eRound { WAIT, BATTLE, FINISH, MAXSIZE };
    public eRound _round;
    private eRound _prevRound;
    private Dictionary<eRound, Round> roundMap;

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

        //캐릭터 구매 UI 초기화(제거 예정)
        resetBtn.onClick.AddListener(() =>
        {
            {
                SetBuySlot(buySlot1, GetRandomCharacterName());
                SetBuySlot(buySlot2, GetRandomCharacterName());
                SetBuySlot(buySlot3, GetRandomCharacterName());
                SetBuySlot(buySlot4, GetRandomCharacterName());
            }
        });

        //최신 플레이어 배치 리스트 저장
        lastPlayerNameList = new List<string>();

        currentRound = 1;
        //round 별 적 데이터 저장
        roundEnemyList = new Dictionary<int, List<eCharacter>>();
        roundEnemyList[1] = new List<eCharacter>();
        roundEnemyList[1].Add(eCharacter.SKELETON);

        roundEnemyList[2] = new List<eCharacter>();
        roundEnemyList[2].Add(eCharacter.ROYALKNIGHT);

        roundEnemyList[3] = new List<eCharacter>();
        roundEnemyList[3].Add(eCharacter.PLASMADRONE);

        roundEnemyList[4] = new List<eCharacter>();
        roundEnemyList[4].Add(eCharacter.TAURUS);

        roundEnemyList[5] = new List<eCharacter>();
        roundEnemyList[5].Add(eCharacter.VEX);

        roundEnemyList[6] = new List<eCharacter>();
        roundEnemyList[6].Add(eCharacter.SANTA);

        roundEnemyList[7] = new List<eCharacter>();
        roundEnemyList[7].Add(eCharacter.SAPCECADET);

        roundMap = new Dictionary<eRound, Round>();
        roundMap[eRound.WAIT] = new WaitRound();
        roundMap[eRound.BATTLE] = new BattleRound();
        roundMap[eRound.FINISH] = new FinishRound();

        for (eRound i = 0; i< eRound.MAXSIZE; i++)
        {
            roundMap[i].InitState();
        }
        _round = eRound.WAIT;
        //초기 세팅만 startState 호출(이후 변경으로 인한 내용은 update에서 감지)
        roundMap[_round].StartState();
        _prevRound = _round;
    }

    public void SaveData()
    {
        SaveRound();
    }

    private void SaveRound()
    {
        Repository.round = currentRound;
    }

    public void SavePlayerList()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                ChessTile chessTile = tilemap.GetTile<ChessTile>(new Vector3Int(j, i, 0));
                if (chessTile)
                {
                    if (chessTile.gameObject)
                    {
                        if (chessTile.gameObject.name.Contains("Player"))
                        {
                            lastPlayerNameList.Add(chessTile.gameObject.name.Split('(')[0]);
                        }
                    }
                }
            }
        }
        
    }

    public void NextRound()
    {
        currentRound++;
        if (currentRound > maxRound)
        {
            currentRound--;
            SaveData();
            //@결과 화면 이동
        }
    }

    public void SpawnEnemies()
    {
        List<GameManager.eCharacter> enemyList = roundEnemyList[currentRound];
        for (int i = 0; i < enemyList.Count; i++)
        {
            SpawnCharacter("Prefabs/Character/" + enemyList[i], "Skeleton(NPC)", 4, 4, false, ChessCharacter.eCharacterType.ENEMY);
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

        //UpdateInput();

        UpdateRound();
        
    }
    
    public bool IsFinishRound()
    {
        int enemyCount = 0;
        int playerCount = 0;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                ChessTile chessTile = tilemap.GetTile<ChessTile>(new Vector3Int(j, i, 0));
                if (chessTile)
                {
                    if (chessTile.gameObject)
                    {
                        if (chessTile.gameObject.name.Contains("NPC"))
                        {
                            enemyCount++;
                        }
                        if (chessTile.gameObject.name.Contains("Player"))
                        {
                            playerCount++;
                        }
                    }
                }
            }
        }
        // 플레이어, 적 중 하나라로 0명이면 라운드 종료
        return playerCount == 0 || enemyCount == 0;
    }

    public bool RoundWin()
    {
        int enemyCount = 0;
        int playerCount = 0;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                ChessTile chessTile = tilemap.GetTile<ChessTile>(new Vector3Int(j, i, 0));
                if (chessTile)
                {
                    if (chessTile.gameObject)
                    {
                        if (chessTile.gameObject.name.Contains("NPC"))
                        {
                            enemyCount++;
                        }
                        if (chessTile.gameObject.name.Contains("Player"))
                        {
                            playerCount++;
                        }
                    }
                }
            }
        }
        // 플레이어 0명이 아니고 적이 없으면 승리
        return playerCount != 0 && enemyCount == 0;
    }

    private void UpdateRound()
    {
        //State update
        if (_round != _prevRound)
        {
            //Debug.Log(this.name + " : end prev state: " + _prevState);
            //Debug.Log(this.name + " : start cur state : " + _state);
            roundMap[_prevRound].EndState();
            roundMap[_round].StartState();
            _prevRound = _round;
        }
        roundMap[_round].UpdateState();
    }

    public void UpdateInput()
    {
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
                        SpawnCharacter("Prefabs/Character/" + nameList[1], nameList[1] + "(Player)", mouseUpTile.GetTilePosition().x, mouseUpTile.GetTilePosition().y, false, ChessCharacter.eCharacterType.PLAYER);
                        //Debug.Log("=start====================");
                        //AllTilesLog();
                        //Debug.Log("=end======================");
                        Destroy(holdTarget);
                        //mouseDownTile.gameObject = holdTarget;
                        //mouseDownTile.gameObject.transform.position = tilemap.layoutGrid.CellToWorld(mouseDownTile.position);
                    }
                    else if (mouseUpTile.GetTilePosition().y == -1)    //대기 타일 내 이동
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

    public void ResetBuyCharacters()
    {
        SetBuySlot(buySlot1, GetRandomCharacterName());
        SetBuySlot(buySlot2, GetRandomCharacterName());
        SetBuySlot(buySlot3, GetRandomCharacterName());
        SetBuySlot(buySlot4, GetRandomCharacterName());
    }

    private string GetRandomCharacterName()
    {
        string name = "";
        eCharacter enumCharacter = (eCharacter)Random.Range(0, (int)eCharacter.MAX);
        return GetCharacterName(enumCharacter);
    }

    public string GetCharacterName(eCharacter enumCharacter)
    {
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

    public void StopBuySlots()
    {
        buySlot1.gameObject.SetActive(false);
        buySlot1.onClick.RemoveAllListeners();
        buySlot2.gameObject.SetActive(false);
        buySlot2.onClick.RemoveAllListeners();
        buySlot3.gameObject.SetActive(false);
        buySlot3.onClick.RemoveAllListeners();
        buySlot4.gameObject.SetActive(false);
        buySlot4.onClick.RemoveAllListeners();
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
            bool success = BuyWaitCharacter("Prefabs/WaitCharacter/Wait" + name, "WaitCharacter_" + name);
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
                    Debug.Log("TEST path : " + path);
                    GameObject character = Instantiate(Resources.Load(path)) as GameObject;
                    character.name = name;
                    ChessCharacter cCharacter = character.GetComponent<ChessWaitCharacter>();
                    character.transform.SetParent(tilemap.transform);

                    cCharacter.SetTilePosition(new Vector3Int(waitPositionX, -1, 0));
                    cCharacter.SetCharacterType(ChessCharacter.eCharacterType.WAIT);
                    chessTile.gameObject = character;
                    return true;
                }
            }
            waitPositionX++;
        }
        return false;
    }

    public void SpawnCharacter(string path, string name, int tileX, int tileY, bool isPlayer, ChessCharacter.eCharacterType characterType)
    {
        GameObject character = Instantiate(Resources.Load(path)) as GameObject;
        character.name = name;
        ChessCharacter cCharacter = character.GetComponent<ChessCharacter>();
        cCharacter.SetTilePosition(new Vector3Int(tileX, tileY, 0));
        cCharacter.IsPlayer(isPlayer);
        cCharacter.SetCharacterType(characterType);
        Debug.Log("Spawn in GameManager");
        character.transform.SetParent(tilemap.transform);

        ChessTile chessTile = tilemap.GetTile<ChessTile>(new Vector3Int(tileX, tileY, 0));
        if (chessTile != null)
        {
            chessTile.gameObject = character;
        }

        GameObject canvas = GameObject.Find("Canvas");
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        //slider transform 세팅
        GameObject sliderObject = Instantiate(Resources.Load("Prefabs/UI/HpBar")) as GameObject;
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