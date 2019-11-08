using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

public class StartController : MonoBehaviour
{
    public AudioClip buttonSe;
    public AudioClip startBgm;
    public GameObject nickAlert;
    public GameObject bgm, se;
    public GameObject loginLayout;
    public GameObject enterButton;

    Text alertMessage;
    bool bWaitingForAuth = false;

    private void Awake()
    {
        DontDestroyOnLoad(bgm);
        DontDestroyOnLoad(se);

        GooglePlayGameServiceManager.Init();
    }

    void Start()
    {
        bgm.GetComponent<AudioSource>().clip = startBgm;
        bgm.GetComponent<AudioSource>().Play();
        alertMessage = nickAlert.GetComponent<Text>();

        AutoLogin();

        if (PlayerPrefs.GetInt("BGM", 1) == 0)
        {
            bgm.GetComponent<AudioSource>().mute = true;
        }

        if (PlayerPrefs.GetInt("SE", 1) == 0)
        {
            se.GetComponent<AudioSource>().mute = true;
        }
    }

    public void AutoLogin()
    {
        ShowAlert("로그인 시도중");
        if (bWaitingForAuth) return;

        if (!Social.localUser.authenticated)
        {
            ShowAlert("로그인 중입니다");
            bWaitingForAuth = true;

            Social.localUser.Authenticate(AuthenticateCallback);
        }
        else
        {
            ShowAlert("로그인에 실패했습니다");
        }
    }

    public void Login()
    {
        se.GetComponent<AudioSource>().clip = buttonSe;
        se.GetComponent<AudioSource>().Play();

        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                GooglePlayGameServiceManager.LoadFromCloud();
                Debug.Log("USERNAME : " + Social.localUser.userName);
                ShowAlert("");
                gameObject.SetActive(false);
                enterButton.SetActive(true);
            }
            else
            {
                ShowAlert("로그인에 실패했습니다");
            }
        });
    }

    public void EnterButton()
    {
        se.GetComponent<AudioSource>().clip = buttonSe;
        se.GetComponent<AudioSource>().Play();

        if (Repository.fLoading)
        {
            if (Repository.sData.ContainsKey("Nickname"))
            {
                SceneManager.LoadScene("LobbyScene");
            }
            else
            {
                ShowAlert("");
                enterButton.SetActive(false);
                loginLayout.SetActive(true);
            }
        }
        else
        {
            ShowAlert("데이터를 불러오는 중입니다");
        }
    }

    public void ToLobby(Text nickname)
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
        else if (Social.localUser.authenticated)
        {
            GooglePlayGameServiceManager.SaveToCloud("Nickname," + nickname.text);
            GooglePlayGameServiceManager.LoadFromCloud();
            Debug.Log(Repository.sData["Nickname"]);
            SceneManager.LoadScene("LobbyScene");
        }
    }

    void AuthenticateCallback(bool success, string error)
    {
        if (success)
        {
            GooglePlayGameServiceManager.LoadFromCloud();
            Debug.Log("USERNAME : " + Social.localUser.userName);
        }
        else if (error == "Not implemented on this platform")
        {
            Repository.sData["Nickname"] = "TEST";
            Repository.sData["Record"] = "32";
            Repository.fLoading = true;
        }
        else
        {
            Debug.Log("오류 : " + error);
            ShowAlert(error);
            return;
        }

        ShowAlert("");
        gameObject.SetActive(false);
        enterButton.SetActive(true);
    }

    void ShowAlert(string message)
    {
        alertMessage.text = message;
        nickAlert.SetActive(true);
    }
}
