using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonFunction : MonoBehaviour
{
    public AudioClip buttonSe;

    private AudioSource se;

    void Start()
    {
        se = GameObject.Find("SE").GetComponent<AudioSource>();
    }

    public void ToInGameScene()
    {
        Repository.isInfinite = GetComponentInChildren<Toggle>().isOn;
        PlaySe();
        Debug.Log(Repository.isInfinite);
        SceneManager.LoadScene("InGameScene");
    }

    public void ToLobby()
    {
        PlaySe();
        SceneManager.LoadScene("LobbyScene");
    }

    public void ToGuidebook()
    {
        PlaySe();
        SceneManager.LoadScene("GuidebookScene");
    }

    public void ExitGame()
    {
        PlaySe();
        Application.Quit();
    }

    void PlaySe()
    {
        se.clip = buttonSe;
        se.Play();
    }
}
