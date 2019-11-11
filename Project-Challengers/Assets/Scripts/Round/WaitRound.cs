using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitRound : Round
{
    private float waitTimer;
    private int totalWaitSec;
    public override void InitState()
    {
        base.InitState();
        totalWaitSec = 30;
    }
    public override void StartState()
    {
        base.StartState();
        GameManager.gameInstance.roundInfo.text = "구매와 배치!";

        //캐릭 생성 관련 테스트(적 유닛 생성)
        GameManager.gameInstance.SpawnEnemies();

        //구매 캐릭터 카드
        GameManager.gameInstance.ResetBuyCharacters();
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
        
        //UI
        int remainSec = totalWaitSec - (int)waitTimer;
        GameManager.gameInstance.roundTimer.text = "남은 배치 시간 : " + remainSec;
    }
}
