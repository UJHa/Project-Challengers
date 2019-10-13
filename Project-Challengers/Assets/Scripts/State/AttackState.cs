using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    public override void StartState()
    {
        base.StartState();
        Debug.Log("atk StartState!!!");
        _cCharacter.AttackStart();
    }

    public override void UpdateState()
    {
        base.UpdateState();
        Debug.Log("atk UpdateState!!! + " + _cCharacter.GetDirection());
        Vector3Int nextTilePosition = _cCharacter.GetTilePosition() + _cCharacter.GetDirectionTileNext(_cCharacter.GetDirection());
        GameManager.gameInstance.SendMessageForTile(nextTilePosition); // 인자 추가로 보낼 내용 정하자.
        _cCharacter.AttackUpdate();
    }
}
