using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public GameObject mainCamera;
    public int freq = 3;
    public Text nicknameText, recordText;
    public Tilemap tilemap;

    public GameObject bgmOn, bgmOff;
    public GameObject seOn, seOff;
    public AudioSource bgm, se;

    GameObject character;
    Animator animator;
    DateTime moved = DateTime.Now;
    float moveSpeed = 1.0f;
    bool moving = false;
    Vector3 pos, destination;
    Vector3Int cellpos, target;

    private string[] eCharacter =
    {
        "BlobMinion",
        "Cyclops",
        "Detective",
        "Dwarf",
        "Imp",
        "Knight",
        "Lizard",
        "PlasmaDrone",
        "RoyalKnight",
        "Santa",
        "Skeleton",
        "SpaceCadet",
        "Taurus",
        "Vex"
    };

    private void Awake()
    {
        bgm = GameObject.Find("BGM").GetComponent<AudioSource>();
        se = GameObject.Find("SE").GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        character = Instantiate(Resources.Load<GameObject>("Prefabs/Character/" + eCharacter[UnityEngine.Random.Range(0, eCharacter.Length)]));
        character.transform.position = tilemap.CellToWorld(new Vector3Int(4, 4, 0));
        mainCamera.transform.SetParent(character.transform);

        recordText.text = Repository.GetRecord();
        nicknameText.text = Repository.sData["Nickname"];
        animator = character.GetComponent<Animator>();
        pos = character.transform.position;
        cellpos = tilemap.WorldToCell(character.transform.position);

        if (PlayerPrefs.GetInt("BGM", 1) == 0)
        {
            bgmOn.SetActive(false);
            bgmOff.SetActive(true);
        }

        if (PlayerPrefs.GetInt("SE", 1) == 0)
        {
            seOn.SetActive(false);
            seOff.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            character.transform.position = Vector3.MoveTowards(character.transform.position, destination, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(character.transform.position, destination) == 0.0f)
            {
                moving = false;
                animator.SetBool("isMoving", false);
                pos = character.transform.position;
                cellpos = tilemap.WorldToCell(character.transform.position);
            }
        }
        else
        {
            if ((DateTime.Now - moved).TotalSeconds >= freq)
            {
                int distance = UnityEngine.Random.Range(1, 3);
                int direction = UnityEngine.Random.Range(0, 4);
                
                switch (direction)
                {
                    case 0:
                        if (cellpos.x + distance > 6)
                        {
                            distance = 6 - cellpos.x;
                        }
                        target = cellpos + new Vector3Int(distance, 0, 0);
                        character.transform.localScale = new Vector3(1, 1, 1);
                        break;
                    case 1:
                        if (cellpos.x - distance < 3)
                        {
                            distance = cellpos.x - 3;
                        }
                        target = cellpos - new Vector3Int(distance, 0, 0);
                        character.transform.localScale = new Vector3(-1, 1, 1);
                        break;
                    case 2:
                        if (cellpos.y + distance > 6)
                        {
                            distance = 6 - cellpos.y;
                        }
                        target = cellpos + new Vector3Int(0, distance, 0);
                        character.transform.localScale = new Vector3(-1, 1, 1);
                        break;
                    case 3:
                        if (cellpos.y - distance < 3)
                        {
                            distance = cellpos.y - 3;
                        }
                        target = cellpos - new Vector3Int(0, distance, 0);
                        character.transform.localScale = new Vector3(1, 1, 1);
                        break;
                }

                if (distance > 0)
                {
                    destination = tilemap.CellToWorld(target);
                    animator.SetBool("isMoving", true);
                    moving = true;
                }
                Debug.Log("Direction = " + direction + ", Distance = " + distance);
                moved = DateTime.Now;
            }
        }
    }
}
