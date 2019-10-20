using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartController : MonoBehaviour
{
    public Text nickname;
    public GameObject nickAlert;

    Text alertMessage;

    void Start()
    {
        alertMessage = nickAlert.GetComponent<Text>();
    }

    public void ToLobby()
    {
        if (nickname.text == "")
        {
            ShowAlert("닉네임을 입력해주세요");
        }
        else if (nickname.text.Length > 6)
        {
            ShowAlert("6글자 이내로 입력해주세요");
        }
        else
        {
            PlayerPrefs.SetString("Nickname", nickname.text);
            PlayerPrefs.Save();
            SceneManager.LoadScene("LobbyScene");
        }
    }

    void ShowAlert(string message)
    {
        alertMessage.text = message;
        nickAlert.SetActive(true);
    }
}
