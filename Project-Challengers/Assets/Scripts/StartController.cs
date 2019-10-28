using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartController : MonoBehaviour
{
    public AudioClip buttonSe;
    public AudioClip startBgm;
    public Text nickname;
    public GameObject nickAlert;
    public GameObject bgm, se;

    Text alertMessage;

    private void Awake()
    {
        DontDestroyOnLoad(bgm);
        DontDestroyOnLoad(se);
    }

    void Start()
    {
        bgm.GetComponent<AudioSource>().clip = startBgm;
        bgm.GetComponent<AudioSource>().Play();
        alertMessage = nickAlert.GetComponent<Text>();
    }

    public void ToLobby()
    {
        se.GetComponent<AudioSource>().clip = buttonSe;
        se.GetComponent<AudioSource>().Play();

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
