using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunction : MonoBehaviour
{
    public void ToInGameScene()
    {
        SceneManager.LoadScene("InGameScene");
    }

    public void ToLobby()
    {
        SceneManager.LoadScene("LobbyScene");
    }
}
