using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : State
{
    private Vector3Int _targetPosition;
    public override void StartState()
    {
        base.StartState();
        _targetPosition = _cCharacter.GetTargetPosition();
        Debug.Log("_targetPosition : " + _targetPosition);
        Debug.Log("_targetPosition convert : " + GameManager.gameInstance.tilemap.layoutGrid.CellToWorld(_targetPosition));
        Debug.Log("_cCharacter.GetTilePosition(); : " + _cCharacter.GetTilePosition());
        Debug.Log("_cCharacter.GetTilePosition()convert : " + GameManager.gameInstance.tilemap.layoutGrid.CellToWorld(_cCharacter.GetTilePosition()));
    }

    public override void UpdateState()
    {
        base.UpdateState();
    }

    public override void EndState()
    {
        base.EndState();
    }
}
