using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartController : MonoBehaviour
{
    public Text nickname;
    public GameObject nickAlert;

    public void ToLobby()
    {
        if (nickname.text != "")
        {
            PlayerPrefs.SetString("Nickname", nickname.text);
            PlayerPrefs.Save();
            SceneManager.LoadScene("LobbyScene");
        }
        else
        {
            nickAlert.SetActive(true);
        }
    }
}
