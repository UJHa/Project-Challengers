using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleRound : Round
{
    private float waitTimer;
    private int totalWaitSec;
    public override void InitState()
    {
        base.InitState();
        totalWaitSec = 10;
    }
    public override void StartState()
    {
        base.StartState();
        GameManager.gameInstance.roundInfo.text = "전투!";
    }

    public override void UpdateState()
    {
        base.UpdateState();
        if (waitTimer >= 30.0f)
        {
            GameManager.gameInstance._round = GameManager.eRound.BATTLE;

            waitTimer = 0.0f;
        }
        waitTimer += Time.deltaTime;
        int remainSec = totalWaitSec - (int)waitTimer;
        GameManager.gameInstance.roundTimer.text = "남은 전투 시간 : " + remainSec;
    }
}
