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
        KNIGHT,
        LIZARD,
        SKELETON,
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
                chessTile.position = new Vector3Int(j, i, 0);
                tilemap.SetTile(chessTile.position, chessTile);
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
            //SpawnCharacter("Prefabs/Skeleton", "Player", 0, 3, true);   //player
            //SpawnCharacter("Prefabs/Knight", "Knight(NPC)", 1, 3, false);
            //SpawnCharacter("Prefabs/Lizard", "Lizard(NPC)", 2, 3, false);
            //SpawnCharacter("Prefabs/Skeleton", "Skeleton(NPC)", 3, 3, false);
            //SpawnCharacter("Prefabs/Knight", 3, 3, false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (tilemap != null)
        {
            tilemap.RefreshAllTiles();
        }
    }

    private string GetRandomCharacter()
    {
        string name = "";
        eCharacter enumCharacter = (eCharacter)Random.Range(0, (int)eCharacter.MAX);
        switch (enumCharacter)
        {
            case eCharacter.KNIGHT:
                name = "Knight";
                break;
            case eCharacter.LIZARD:
                name = "Lizard";
                break;
            case eCharacter.SKELETON:
                name = "Skeleton";
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
            bool success = BuyWaitCharacter("Prefabs/Wait" + name, "WaitCharacter(" + name + ")");
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

                    //캐릭터 UI
                    GameObject canvas = GameObject.Find("Canvas");
                    RectTransform canvasRect = canvas.GetComponent<RectTransform>();

                    //slider transform 세팅
                    GameObject sliderObject = Instantiate(Resources.Load("Prefabs/HpBar")) as GameObject;
                    sliderObject.transform.SetParent(canvas.transform, false);

                    Slider slider = sliderObject.GetComponent<Slider>();
                    cCharacter.SetHpBar(slider);
                    sliderObject.SetActive(false);
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
                chessTile.prevPathTileNodeMap[keyName] = null;
            }
        }
    }
}