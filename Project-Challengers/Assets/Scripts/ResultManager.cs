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
        if (round > int.Parse(Repository.record))
        {
            Repository.UpdateData("Record", round.ToString());
        }

        nicknameText.text = Repository.sData["Nickname"];
        resultText.text = round + " 단계 달성!";
        recordText.text = Repository.GetRecord();
    }
}
