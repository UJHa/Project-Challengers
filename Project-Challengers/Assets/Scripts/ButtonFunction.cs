using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        PlaySe();
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

    public void PlaySe()
    {
        se.clip = buttonSe;
        se.Play();
    }
}
