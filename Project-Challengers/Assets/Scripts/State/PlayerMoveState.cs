using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMoveState : State
{
    ChessTile targetTile;
    private enum ePathState
    {
        PATH_FIND,
        PATH_MOVE
    }
    ePathState pathState;

    public override void InitState(ChessCharacter cCharacter)
    {
        base.InitState(cCharacter);
    }

    public override void StartState()
    {
        base.StartState();
        Debug.Log("[" + _cCharacter.name + "]Player move Start State");
        pathState = ePathState.PATH_FIND;
        
        _cCharacter.GetAnimator().SetBool("isMoving", true);

        _cCharacter.ClearPathFindQueue();
        _cCharacter.ClearPathStack();
        GameManager.gameInstance.ResetTilePath(_cCharacter.name);

        Debug.Log("[" + _cCharacter.name + "]Move target : " + _cCharacter.GetMoveTarget().GetTilePosition());
        FindPath(_cCharacter.GetMoveTarget().GetTilePosition());
    }
    private ChessTile aimTile;
    public override void UpdateState()
    {
        base.UpdateState();

        if (pathState == ePathState.PATH_FIND)
        {
            Debug.Log("[" + _cCharacter.name + "]ePathState.PATH_FIND");
            // 경로대로 이동 종료
            if (_cCharacter.GetTilePosition() == _cCharacter.GetMoveTarget().GetTilePosition())   //목표 == 현재 내 위치일 때
            {
                Debug.Log("[" + _cCharacter.name + "] 목표 도착");
                _cCharacter.SetState(ChessCharacter.eState.IDLE);
                return;
            }

            if (targetTile.GetTilePosition() != _cCharacter.GetTilePosition())
            {
                Debug.Log("적 위치 변경으로 재탐색");
                _cCharacter.SetState(ChessCharacter.eState.IDLE);
                return;
            }
            //경로 상의 다음 타일 세팅
            aimTile = _cCharacter.PopPathStackTile();

            if (aimTile != null && _cCharacter.CanMoveTile(aimTile.GetTilePosition()))
            {
                //Debug.Log("ePathState.PATH_FIND canMove");
                //이동 방향 설정
                Vector3Int dirctionVector = aimTile.GetTilePosition() - _cCharacter.GetTilePosition();
                ChessCharacter.Direction direction = ChessCharacter.Direction.RIGHT;
                if (dirctionVector == Vector3Int.up)
                {
                    direction = ChessCharacter.Direction.UP;
                }
                else if (dirctionVector == Vector3Int.down)
                {
                    direction = ChessCharacter.Direction.DOWN;
                }
                else if (dirctionVector == Vector3Int.left)
                {
                    direction = ChessCharacter.Direction.LEFT;
                }
                else if (dirctionVector == Vector3Int.right)
                {
                    direction = ChessCharacter.Direction.RIGHT;
                }
                _cCharacter.SetDirection(direction);

                //이동하는 타일로 캐릭 정보 이동
                GameManager.gameInstance.tilemap.SetColliderType(_cCharacter.GetTilePosition(), Tile.ColliderType.None);
                GameManager.gameInstance.SetTileObject(_cCharacter.GetTilePosition(), null);
                _cCharacter.SetTilePosition(aimTile.GetTilePosition());
                pathState = ePathState.PATH_MOVE;
            }
            else
            {
                Debug.Log("ePathState.PATH_FIND can't Move");
                // 다음 이동 타일에 배치된 오브젝트 있으면 그 대상 공격하기
                if (aimTile != null && aimTile.gameObject != null)
                {
                    ChessCharacter attackTargetCharacter = aimTile.gameObject.GetComponent<ChessCharacter>();
                    if (attackTargetCharacter != null)
                    {
                        if (_cCharacter.GetCharacterType() != attackTargetCharacter.GetCharacterType() && attackTargetCharacter.GetCharacterType() != ChessCharacter.eCharacterType.WAIT) // 적일 때만 공격 시킨다.
                        {
                            _cCharacter.SetAttackTarget(attackTargetCharacter);
                            _cCharacter.SetState(ChessCharacter.eState.ATTACK);
                        }
                    }
                }
                else
                {
                    _cCharacter.SetState(ChessCharacter.eState.IDLE);
                }
            }
        }
        else if (pathState == ePathState.PATH_MOVE)
        {
            //Debug.Log("ePathState.PATH_MOVE");
            //현재 타일 > 다음 타일로의 이동
            Vector3 aimPosition = GameManager.gameInstance.tilemap.layoutGrid.CellToWorld(aimTile.GetTilePosition());
            _cCharacter.transform.position = Vector3.MoveTowards(_cCharacter.transform.position, aimPosition, _cCharacter.moveSpeed * Time.deltaTime);
            if (Vector3.Distance(_cCharacter.transform.position, aimPosition) == 0.0f)
            {
                pathState = ePathState.PATH_FIND;
            }
        }
    }

    public override void EndState()
    {
        base.EndState();
        _cCharacter.GetAnimator().SetBool("isMoving", false);

        GameManager.gameInstance.ResetTilePath(_cCharacter.name);
        _cCharacter.SetMoveTarget(null);

        _cCharacter.ClearPathFindQueue();
        _cCharacter.ClearPathStack();
    }

    private void FindPath(Vector3Int targetTilePosition)
    {
        _cCharacter.ClearPathFindQueue();
        _cCharacter.ClearPathStack();
        GameManager.gameInstance.ResetTilePath(_cCharacter.name);

        ChessTile startTile;
        startTile = GameManager.gameInstance.tilemap.GetTile<ChessTile>(_cCharacter.GetTilePosition());
        startTile.SetPrevPathTileNodeMap(_cCharacter.name, startTile);
        _cCharacter.PushPathFindTile(startTile);

        ChessTile nextQueueTile = _cCharacter.PopPathFindTile();
        while (nextQueueTile != null)
        {
            ChessTile currentTile = nextQueueTile;

            //목표 타일에 도달하면 반환
            if (currentTile.GetTilePosition() == targetTilePosition)
            {
                ChessTile pathTile = currentTile;
                while (pathTile.GetPrevPathTileNodeMap(_cCharacter.name) != null && pathTile.GetPrevPathTileNodeMap(_cCharacter.name) != pathTile)
                {
                    _cCharacter.PushPathStackTile(pathTile);
                    pathTile = pathTile.GetPrevPathTileNodeMap(_cCharacter.name);
                    targetTile = pathTile;
                }
                Debug.Log("@@@@@path finish1!!!");
                return;
            }
            else
            {
                for (int i = 0; i < (int)ChessCharacter.Direction.MAXSIZE; i++)
                {
                    ChessCharacter.Direction direction = (ChessCharacter.Direction)i;
                    Vector3Int nextTilePos = currentTile.GetTilePosition() + _cCharacter.GetDirectionTileNext(direction);
                    ChessTile nextTile = GameManager.gameInstance.tilemap.GetTile<ChessTile>(nextTilePos);
                    if (_cCharacter.IsInWall(nextTilePos) && nextTile.GetPrevPathTileNodeMap(_cCharacter.name) == null)
                    {
                        nextTile.SetPrevPathTileNodeMap(_cCharacter.name, currentTile);
                        //Debug.Log("nextTile : " + nextTile.position + "direction : " + direction);
                        _cCharacter.PushPathFindTile(nextTile);
                    }
                }
            }
            nextQueueTile = _cCharacter.PopPathFindTile();
        }
    }
}
