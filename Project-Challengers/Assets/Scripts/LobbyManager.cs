
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public int freq = 3;
    public GameObject character;
    public Text nicknameText, recordText;
    public Tilemap tilemap;

    public GameObject bgmOn, bgmOff;
    public GameObject _bgm;

    Animator animator;
    DateTime moved = DateTime.Now;
    Boolean moving = false;
    Vector3 pos, destination;
    Vector3Int cellpos, target;
    AudioSource bgm;

    // Start is called before the first frame update
    void Start()
    {
        recordText.text = Repository.GetRecord();
        nicknameText.text = PlayerPrefs.GetString("Nickname");
        animator = character.GetComponent<Animator>();
        pos = character.transform.position;
        cellpos = tilemap.WorldToCell(character.transform.position);
        bgm = _bgm.GetComponent<AudioSource>();

        if (PlayerPrefs.GetInt("BGM", 1) == 0)
        {
            bgm.mute = true;
            bgmOn.SetActive(false);
            bgmOff.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            character.transform.position += (destination - pos) * Time.deltaTime;
            if (Vector3.Distance(character.transform.position, destination) < 0.1f)
            {
                character.transform.position = destination;
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
