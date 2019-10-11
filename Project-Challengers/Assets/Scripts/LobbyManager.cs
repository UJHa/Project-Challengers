using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public Text nicknameText;

    // Start is called before the first frame update
    void Start()
    {
        nicknameText.text = PlayerPrefs.GetString("Nickname");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
