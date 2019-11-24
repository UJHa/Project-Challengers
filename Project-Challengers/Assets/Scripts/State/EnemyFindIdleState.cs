using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFindIdleState : State
{
    Queue<ChessTile> findQueue;

    public override void InitState(ChessCharacter cCharacter)
    {
        base.InitState(cCharacter);
        findQueue = new Queue<ChessTile>();
    }

    public override void StartState()
    {
        base.StartState();

        findQueue.Clear();
        GameManager.gameInstance.ResetTilePath(_cCharacter.name);
    }

    public override void UpdateState()
    {
        base.UpdateState();
        bool stateChange = ChangeState(_cCharacter._attackRange, ChessCharacter.eState.ATTACK);
        if (stateChange) return;
        stateChange = ChangeState(_cCharacter._findRange, ChessCharacter.eState.MOVE);
        if (stateChange) return;
    }

    public override void EndState()
    {
        base.EndState();
        GameManager.gameInstance.ResetTilePath(_cCharacter.name);
    }

    private bool ChangeState(int range, ChessCharacter.eState state)
    {
        ChessCharacter targetCharacter = FindAdjacentTarget(range);
        if (targetCharacter != null)
        {
            if (range <= _cCharacter._attackRange)
            {
                _cCharacter.SetAttackTarget(targetCharacter);
            }
            else
            {
                _cCharacter.SetTargetTilePosition(targetCharacter.GetTilePosition()/*타일 좌표 0,0~7,7사이*/);
            }
            _cCharacter.SetState(state);
            return true;
        }
        return false;
    }

    private ChessCharacter FindAdjacentTarget(int findRange)
    {
        findQueue.Clear();
        GameManager.gameInstance.ResetTilePath(_cCharacter.name);

        ChessTile startTile;
        startTile = GameManager.gameInstance.tilemap.GetTile<ChessTile>(_cCharacter.GetTilePosition());
        startTile.SetPrevPathTileNodeMap(_cCharacter.name, startTile);
        findQueue.Enqueue(startTile);
        while (findQueue.Count > 0)
        {
            ChessTile currentTile = findQueue.Dequeue();
            if (currentTile == null)
            {
                break;
            }

            if (currentTile.gameObject != null && currentTile.GetDistanceWeight() != 0)
            {
                ChessCharacter targetCharacter = currentTile.gameObject.GetComponent<ChessCharacter>();
                if (_cCharacter.GetCharacterType() != targetCharacter.GetCharacterType() && targetCharacter.GetCharacterType() != ChessCharacter.eCharacterType.WAIT) // 적일 때만 탐색 성공 시킨다.
                {
                    return targetCharacter;
                }
            }
            else if (currentTile.GetDistanceWeight() == findRange + 1)
            {
                return null;
            }
            else
            {
                for (int i = 0; i < (int)ChessCharacter.Direction.MAXSIZE; i++)
                {
                    ChessCharacter.Direction direction = (ChessCharacter.Direction)i;
                    Vector3Int nextTilePos = currentTile.GetTilePosition() + _cCharacter.GetDirectionTileNext(direction);
                    ChessTile nextTile = GameManager.gameInstance.tilemap.GetTile<ChessTile>(nextTilePos);
                    if (_cCharacter.IsInWall(nextTilePos) //맵 안에 있을 때 분기
                        && nextTile.GetPrevPathTileNodeMap(_cCharacter.name) == null) // 이미 이전 타일 세팅 안되있을 때 분기
                    {
                        nextTile.SetPrevPathTileNodeMap(_cCharacter.name, currentTile);
                        nextTile.SetDistanceWeight(currentTile.GetDistanceWeight() + 1);
                        findQueue.Enqueue(nextTile);
                    }
                }
            }
        }
        return null;
    }
}
