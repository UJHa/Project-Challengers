using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleRound : Round
{
    public override void InitState()
    {
        base.InitState();
    }
    public override void StartState()
    {
        base.StartState();
        //UI
        GameManager.gameInstance.roundInfo.text = "전투!";
        GameManager.gameInstance.roundTimer.text = "전투 진행중...";

        //구매 캐릭터 카드 비활성화
        GameManager.gameInstance.StopBuySlots();

        //플레이어 최신 배치 리스트 저장
        GameManager.gameInstance.SavePlayerList();
    }

    public override void UpdateState()
    {
        base.UpdateState();
        if (GameManager.gameInstance.IsFinishRound())
        {
            GameManager.gameInstance._round = GameManager.eRound.FINISH;
        }
    }
}
