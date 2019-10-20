using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    public Text nicknameText;
    public Text resultText, recordText;
    public int round;

    // Start is called before the first frame update
    void Start()
    {
        nicknameText.text = PlayerPrefs.GetString("Nickname");
        resultText.text = round + " ROUND 달성!";
        recordText.text = "최고 기록 " + PlayerPrefs.GetInt("Record", 0) + " ROUND";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
