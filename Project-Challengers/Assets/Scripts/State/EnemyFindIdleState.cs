using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFindIdleState : State
{
    private float findTimer;

    Queue<ChessTile> findQueue;

    public override void InitState(ChessCharacter cCharacter)
    {
        base.InitState(cCharacter);
        findQueue = new Queue<ChessTile>();
    }

    public override void StartState()
    {
        base.StartState();
        findTimer = 0.0f;

        findQueue.Clear();
        GameManager.gameInstance.ResetTilePath(_cCharacter.name);
    }

    public override void UpdateState()
    {
        base.UpdateState();
        if (findTimer >= 1.0f) // 1초에 한번 씩 주변 범위 적 탐색
        {
            ChessCharacter attackTargetCharacter = FindAdjacentTarget(_cCharacter._attackRange);
            if (attackTargetCharacter != null)
            {
                _cCharacter.SetAttackTarget(attackTargetCharacter);
                _cCharacter.SetState(ChessCharacter.eState.ATTACK);
            }
            //이동 체크
            ChessCharacter moveTargetCharacter = FindAdjacentTarget(_cCharacter._findRange);
            if (moveTargetCharacter != null)
            {
                //Debug.Log("["+_cCharacter.name+"]IDLE target : " + moveTargetCharacter.GetTilePosition());
                _cCharacter.SetTargetTilePosition(moveTargetCharacter.GetTilePosition()/*타일 좌표 0,0~7,7사이*/);
                _cCharacter.SetState(ChessCharacter.eState.MOVE);
            }

            findTimer = 0.0f;
        }
        findTimer += Time.deltaTime;
    }

    public override void EndState()
    {
        base.EndState();
        findTimer = 0.0f;
        GameManager.gameInstance.ResetTilePath(_cCharacter.name);
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
                ChessCharacter character = currentTile.gameObject.GetComponent<ChessCharacter>();
                if (_cCharacter.GetCharacterType() != character.GetCharacterType()) // 적일 때만 탐색 성공 시킨다.
                {
                    return character;
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
