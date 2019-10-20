using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public GameObject _bgm;
    public GameObject _bgmon;
    public GameObject _bgmoff;

    private AudioSource sound;

    private void Start()
    {
        sound = _bgm.GetComponent<AudioSource>();
    }

    public void BgmOff()
    {
        sound.mute = true;
        PlayerPrefs.SetInt("BGM", 0);
        PlayerPrefs.Save();
        _bgmon.SetActive(false);
        _bgmoff.SetActive(true);
    }

    public void BgmOn()
    {
        sound.mute = false;
        PlayerPrefs.SetInt("BGM", 1);
        PlayerPrefs.Save();
        _bgmon.SetActive(true);
        _bgmoff.SetActive(false);
    }
}
