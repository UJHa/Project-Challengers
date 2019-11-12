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

    private struct sSpawnCharacter
    {
        public eCharacter character;
        public int tileX;
        public int tileY;
    }

    private Dictionary<int, List<sSpawnCharacter>> roundEnemyList;
    private List<sSpawnCharacter> lastPlayerNameList;
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
        SPACECADET,
        TAURUS,
        MAX,
        WORM,
        DARKKNIGHT
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
                SetBuySlot(buySlot1, GetRandomCharacter());
                SetBuySlot(buySlot2, GetRandomCharacter());
                SetBuySlot(buySlot3, GetRandomCharacter());
                SetBuySlot(buySlot4, GetRandomCharacter());
            }
        });

        //최신 플레이어 배치 리스트 저장
        lastPlayerNameList = new List<sSpawnCharacter>();

        currentRound = 1;
        //round 별 적 데이터 저장
        roundEnemyList = new Dictionary<int, List<sSpawnCharacter>>();
        roundEnemyList[1] = new List<sSpawnCharacter>();
        roundEnemyList[1].Add(GetSpawnCharacterInfo(eCharacter.ROYALKNIGHT, 2, 4));
        //roundEnemyList[1].Add(GetSpawnCharacterInfo(eCharacter.ROYALKNIGHT, 2, 5));
        //roundEnemyList[1].Add(GetSpawnCharacterInfo(eCharacter.SKELETON, 5, 4));
        //roundEnemyList[1].Add(GetSpawnCharacterInfo(eCharacter.SKELETON, 5, 5));

        roundEnemyList[2] = new List<sSpawnCharacter>();
        roundEnemyList[2].Add(GetSpawnCharacterInfo(eCharacter.WORM, 0, 7));
        roundEnemyList[2].Add(GetSpawnCharacterInfo(eCharacter.WORM, 1, 7));
        roundEnemyList[2].Add(GetSpawnCharacterInfo(eCharacter.WORM, 6, 7));
        roundEnemyList[2].Add(GetSpawnCharacterInfo(eCharacter.WORM, 7, 6));
        roundEnemyList[2].Add(GetSpawnCharacterInfo(eCharacter.WORM, 7, 7));

        roundEnemyList[3] = new List<sSpawnCharacter>();
        roundEnemyList[3].Add(GetSpawnCharacterInfo(eCharacter.TAURUS, 2, 5));
        roundEnemyList[3].Add(GetSpawnCharacterInfo(eCharacter.TAURUS, 3, 6));
        roundEnemyList[3].Add(GetSpawnCharacterInfo(eCharacter.BLOBMINION, 4, 6));
        roundEnemyList[3].Add(GetSpawnCharacterInfo(eCharacter.BLOBMINION, 5, 5));

        roundEnemyList[4] = new List<sSpawnCharacter>();
        roundEnemyList[4].Add(GetSpawnCharacterInfo(eCharacter.DWARF, 2, 4));
        roundEnemyList[4].Add(GetSpawnCharacterInfo(eCharacter.DWARF, 3, 4));
        roundEnemyList[4].Add(GetSpawnCharacterInfo(eCharacter.DWARF, 4, 4));
        roundEnemyList[4].Add(GetSpawnCharacterInfo(eCharacter.DWARF, 5, 4));
        roundEnemyList[4].Add(GetSpawnCharacterInfo(eCharacter.DWARF, 6, 4));
        roundEnemyList[4].Add(GetSpawnCharacterInfo(eCharacter.SANTA, 4, 6));

        roundEnemyList[5] = new List<sSpawnCharacter>();
        roundEnemyList[5].Add(GetSpawnCharacterInfo(eCharacter.IMP, 6, 7));
        roundEnemyList[5].Add(GetSpawnCharacterInfo(eCharacter.IMP, 7, 6));
        roundEnemyList[5].Add(GetSpawnCharacterInfo(eCharacter.DARKKNIGHT, 7, 7));

        roundEnemyList[6] = new List<sSpawnCharacter>();
        roundEnemyList[6].Add(GetSpawnCharacterInfo(eCharacter.SANTA, 4, 4));

        roundEnemyList[7] = new List<sSpawnCharacter>();
        roundEnemyList[7].Add(GetSpawnCharacterInfo(eCharacter.SPACECADET, 4, 4));

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

    private sSpawnCharacter GetSpawnCharacterInfo(eCharacter character, int tileX, int tileY)
    {
        sSpawnCharacter sCharacter;
        sCharacter.character = character;
        sCharacter.tileX = tileX;
        sCharacter.tileY = tileY;
        return sCharacter;
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
                            ChessCharacter character = chessTile.gameObject.GetComponent<ChessCharacter>();
                            lastPlayerNameList.Add(GetSpawnCharacterInfo(character.GetChessCharacterType(), chessTile.GetTilePosition().x, chessTile.GetTilePosition().y));
                        }
                    }
                }
            }
        }
    }

    public void DestroyPlayerList()
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
                            Destroy(chessTile.gameObject);
                        }
                    }
                }
            }
        }
    }
    public void SetPlayerList()
    {
        for (int i = 0; i < lastPlayerNameList.Count; i++)
        {
            sSpawnCharacter sCharacter =  lastPlayerNameList[i];
            SpawnCharacter("Prefabs/Character/" + sCharacter.character.ToString(), sCharacter.character.ToString() + "(Player)", sCharacter.tileX, sCharacter.tileY, false, sCharacter.character, ChessCharacter.eCharacterType.PLAYER);
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
        List<sSpawnCharacter> enemyList = roundEnemyList[currentRound];
        for (int i = 0; i < enemyList.Count; i++)
        {
            Debug.Log("name : " + enemyList[i].character);
            SpawnCharacter("Prefabs/Character/" + enemyList[i].character, enemyList[i].character + "(NPC)", enemyList[i].tileX, enemyList[i].tileY, false, enemyList[i].character, ChessCharacter.eCharacterType.ENEMY);
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
                        if (mouseUpTile.gameObject == null)
                        {
                            ChessWaitCharacter chessWaitCharacter = holdTarget.GetComponent<ChessWaitCharacter>();
                            SpawnCharacter("Prefabs/Character/" + nameList[1], nameList[1] + "(Player)", mouseUpTile.GetTilePosition().x, mouseUpTile.GetTilePosition().y, false, chessWaitCharacter.GetChessCharacterType(), ChessCharacter.eCharacterType.PLAYER);
                            Destroy(holdTarget);
                        }
                        else
                        {
                            Debug.Log("가는 타일에 기존 캐릭터가 있습니다.");
                            mouseDownTile.gameObject = holdTarget;
                            mouseDownTile.gameObject.transform.position = tilemap.layoutGrid.CellToWorld(mouseDownTile.GetTilePosition());
                        }
                        
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
        SetBuySlot(buySlot1, GetRandomCharacter());
        SetBuySlot(buySlot2, GetRandomCharacter());
        SetBuySlot(buySlot3, GetRandomCharacter());
        SetBuySlot(buySlot4, GetRandomCharacter());
    }

    private eCharacter GetRandomCharacter()
    {
        string name = "";
        eCharacter enumCharacter = (eCharacter)Random.Range(0, (int)eCharacter.MAX);
        return enumCharacter;
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
            case eCharacter.SPACECADET:
                name = "SpaceCadet";
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

    private void SetBuySlot(Button buySlot, eCharacter name)
    {
        buySlot.gameObject.SetActive(true);
        //이미지 세팅
        GameObject imageObject = buySlot.gameObject.transform.Find("Image").gameObject;
        Image image = imageObject.GetComponent<Image>();
        image.sprite = Resources.Load<Sprite>("BuySlotImages/" + name.ToString());
        Debug.Log(name.ToString());
        ResizeImage(image, imageObject);

        //이름 세팅
        GameObject nameTextObject = buySlot.gameObject.transform.Find("Text").gameObject;
        Text nameText = nameTextObject.GetComponent<Text>();
        nameText.text = name.ToString();

        buySlot.onClick.RemoveAllListeners();
        buySlot.onClick.AddListener(() => {
            bool success = BuyWaitCharacter("Prefabs/WaitCharacter/Wait" + name, "WaitCharacter_" + name, name);
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
        Debug.Log(imageRect);
        Debug.Log(image.sprite);
        float rateWidth = imageRect.rect.width * (image.sprite.rect.width / image.sprite.rect.height);
        imageRect.sizeDelta = new Vector2(rateWidth, imageRect.rect.height);
    }

    private bool BuyWaitCharacter(string path, string name, eCharacter characterType)
    {
        int waitPositionX = 0;
        while (waitPositionX < 8)
        {
            ChessTile chessTile = tilemap.GetTile<ChessTile>(new Vector3Int(waitPositionX, -1, 0));
            if (chessTile != null)
            {
                if (chessTile.gameObject == null)
                {
                    GameObject character = Instantiate(Resources.Load(path)) as GameObject;
                    character.name = name;
                    ChessCharacter cCharacter = character.GetComponent<ChessWaitCharacter>();
                    character.transform.SetParent(tilemap.transform);

                    cCharacter.SetTilePosition(new Vector3Int(waitPositionX, -1, 0));
                    cCharacter.SetCharacterType(ChessCharacter.eCharacterType.WAIT);
                    cCharacter.SetChessCharacterType(characterType);
                    chessTile.gameObject = character;
                    return true;
                }
            }
            waitPositionX++;
        }
        return false;
    }

    public void SpawnCharacter(string path, string name, int tileX, int tileY, bool isPlayer, eCharacter chessCharacterType, ChessCharacter.eCharacterType characterType)
    {
        GameObject character = Instantiate(Resources.Load(path)) as GameObject;
        character.name = name;
        ChessCharacter cCharacter = character.GetComponent<ChessCharacter>();
        cCharacter.SetTilePosition(new Vector3Int(tileX, tileY, 0));
        cCharacter.IsPlayer(isPlayer);
        cCharacter.SetCharacterType(characterType);
        cCharacter.SetChessCharacterType(chessCharacterType);
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
            //Debug.Log("Tile 범위 초과 : GetTileObject");
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