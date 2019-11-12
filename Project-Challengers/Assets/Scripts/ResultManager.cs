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
    public GameObject unitPanel, unitsGrid;

    private GameObject bgm, se;
    public AudioClip startBgm;

    // Start is called before the first frame update
    void Start()
    {
        bgm = GameObject.Find("bgm");
        se = GameObject.Find("se");
        bgm.GetComponent<AudioSource>().clip = startBgm;
        bgm.GetComponent<AudioSource>().Play();

        //캐릭터 이미지 추가
        foreach(string name in GameManager.gameInstance.lastPlayerNameList)
        {
            GameObject parent = Instantiate(unitPanel, unitsGrid.transform);
            GameObject unit = Instantiate(Resources.Load<GameObject>("Prefabs/WaitCharacter/Wait" + name), parent.transform);
            unit.GetComponent<ChessWaitCharacter>().enabled = false;
            unit.transform.localScale = new Vector3(150, 150);
        }

        Repository.round = GameManager.gameInstance.currentRound;
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
