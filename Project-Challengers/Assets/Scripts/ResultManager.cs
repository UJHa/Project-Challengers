using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    const int MAXLEVEL = 7;

    public Text nicknameText;
    public Text resultText, recordText;

    // Start is called before the first frame update
    void Start()
    {
        nicknameText.text = Repository.sData["Nickname"];

        if (Repository.round >= MAXLEVEL)
        {
            Repository.isClear = true;
            resultText.text = "클리어!";
            recordText.text = "축하드립니다";
        }
        else
        {
            if (Repository.round > int.Parse(Repository.record)) Repository.UpdateData("Record", Repository.round.ToString());

            resultText.text = "패배";
            recordText.text = Repository.round + "단계 달성!";
        }
    }
}
