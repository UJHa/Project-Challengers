using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Sprite[] sprites;
    public GameObject character;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start GameManager!!!");

        sprites = Resources.LoadAll<Sprite>("0x72_16x16DungeonTileset.v4");
        Debug.Log("sprites : "+sprites.Length);
        //cube.transform.position = new Vector3(x, y, 0);
        GameObject player = Instantiate(character);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
