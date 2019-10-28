using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public LobbyManager lobbyManager;
    public GameObject opposite;
    public string type;
    public AudioClip buttonSe;

    private AudioSource sound;
    private AudioSource se;

    private void Start()
    {
        se = lobbyManager.se.GetComponent<AudioSource>();
        se.clip = buttonSe;
        sound = type == "BGM" ? lobbyManager.bgm : lobbyManager.se;
    }

    public void TurnSound(bool turnon)
    {
        sound.mute = !turnon;
        PlayerPrefs.SetInt(type, turnon ? 1 : 0);
        PlayerPrefs.Save();
        se.Play();
        gameObject.SetActive(false);
        opposite.SetActive(true);
    }
}
