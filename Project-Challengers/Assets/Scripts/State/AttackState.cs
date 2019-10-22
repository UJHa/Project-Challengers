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
        _cCharacter.AttackUpdate();
    }

    public override void EndState()
    {
        base.EndState();
        Debug.Log("ATTack END!!!!!");
        //Vector3Int nextTilePosition = _cCharacter.GetTilePosition() + _cCharacter.GetDirectionTileNext(_cCharacter.GetDirection());
        //GameObject gameObject = GameManager.gameInstance.GetTileObject(nextTilePosition);
        //if (gameObject != null)
        //{
        //    ChessCharacter character = gameObject.GetComponent<ChessCharacter>();
        //    Debug.Log("character : " + character);
        //    character.SendMessage("AttackDamage", _cCharacter.GetAttackPower());
        //}
    }
}
