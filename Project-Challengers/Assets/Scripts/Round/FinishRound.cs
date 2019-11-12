using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishRound : Round
{
    private float waitTimer;
    private int totalWaitSec;
    public override void InitState()
    {
        base.InitState();
        totalWaitSec = 5;
    }
    public override void StartState()
    {
        base.StartState();
        //UI
        GameManager.gameInstance.roundInfo.text = "전투 끝!";
        if (GameManager.gameInstance.RoundWin())
        {
            GameManager.gameInstance.roundTimer.text = "승리";
        }
        else
        {
            GameManager.gameInstance.roundTimer.text = "패배";
        }
    }

    public override void UpdateState()
    {
        base.UpdateState();
        if (waitTimer >= totalWaitSec)
        {
            
            if (GameManager.gameInstance.RoundWin())
            {
                GameManager.gameInstance.NextRound();
                GameManager.gameInstance._round = GameManager.eRound.WAIT;
            }
            else
            {
                GameManager.gameInstance.SaveData();
                SceneManager.LoadScene("ResultScene");
            }

            //waitTimer = 0.0f; // 딱 한번만 보여줄 내용
        }
        else
        {
            waitTimer += Time.deltaTime;
        }
    }
}
