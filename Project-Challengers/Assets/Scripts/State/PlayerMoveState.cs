using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMoveState : State
{
    Queue<ChessTile> findQueue;
    Stack<ChessTile> pathStack;
    private enum ePathState
    {
        PATH_FIND,
        PATH_MOVE
    }
    ePathState pathState;

    public override void InitState(ChessCharacter cCharacter)
    {
        base.InitState(cCharacter);
        findQueue = new Queue<ChessTile>();
        pathStack = new Stack<ChessTile>();
    }

    public override void StartState()
    {
        base.StartState();
        Debug.Log("=====Player move Start State");
        pathState = ePathState.PATH_FIND;
        
        _cCharacter.GetAnimator().SetBool("isMoving", true);

        findQueue.Clear();
        pathStack.Clear();
        GameManager.gameInstance.ResetTilePath(_cCharacter.name);

        Vector3Int startPos = _cCharacter.GetTilePosition();
        FindPath(startPos, _cCharacter.GetTargetTilePosition());
    }
    private ChessTile aimTile;
    public override void UpdateState()
    {
        base.UpdateState();

        if (pathState == ePathState.PATH_FIND)
        {
            if (pathStack.Count == 0)
            {
                _cCharacter.SetState(ChessCharacter.eState.IDLE);
                return;
            }
            //경로 상의 다음 타일 세팅
            aimTile = pathStack.Pop();
            //Debug.Log("aimTile : " + aimTile.position);

            if (aimTile != null && _cCharacter.CanMoveTile(aimTile.position))
            {
                Debug.Log("ePathState.PATH_FIND canMove");
                //이동 방향 설정
                Vector3Int dirctionVector = aimTile.position - _cCharacter.GetTilePosition();
                ChessCharacter.Direction direction = ChessCharacter.Direction.RIGHT;
                Debug.Log(dirctionVector);
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
                Debug.Log("direction : " + direction);
                _cCharacter.SetDirection(direction);

                //이동하는 타일로 캐릭 정보 이동
                GameManager.gameInstance.tilemap.SetColliderType(_cCharacter.GetTilePosition(), Tile.ColliderType.None);
                GameManager.gameInstance.SetTileObject(_cCharacter.GetTilePosition(), null);
                _cCharacter.SetTilePosition(aimTile.position);
                pathState = ePathState.PATH_MOVE;
            }
            else
            {
                Debug.Log("ePathState.PATH_FIND can't Move");
                _cCharacter.SetState(ChessCharacter.eState.IDLE);
            }
        }
        else if (pathState == ePathState.PATH_MOVE)
        {
            //Debug.Log("ePathState.PATH_MOVE");
            //현재 타일 > 다음 타일로의 이동
            Vector3 aimPosition = GameManager.gameInstance.tilemap.layoutGrid.CellToWorld(aimTile.position);
            _cCharacter.transform.position = Vector3.MoveTowards(_cCharacter.transform.position, aimPosition, 1.0f/*moveSpeed*/ * Time.deltaTime);
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
    }

    private void FindPath(Vector3Int startTilePosition, Vector3Int targetTilePosition)
    {
        ChessTile startTile;
        startTile = GameManager.gameInstance.tilemap.GetTile<ChessTile>(startTilePosition);
        startTile.prevPathTileNodeMap[_cCharacter.name] = startTile;
        findQueue.Enqueue(startTile);
        while (findQueue.Count > 0)
        {
            ChessTile currentTile = findQueue.Dequeue();
            if (currentTile == null)
            {
                break;
            }

            if (currentTile.position == targetTilePosition)
            {
                // 현재 탐색 타일이 목적 타일인 경우
                ChessTile pathTile = currentTile;
                while (pathTile.prevPathTileNodeMap[_cCharacter.name] != null && pathTile.prevPathTileNodeMap[_cCharacter.name] != pathTile)
                {
                    //Debug.Log("path log : " + pathTile.position);
                    //Debug.Log("path prevPathTileNode : " + pathTile.prevPathTileNode);
                    //pathTile.sprite = null;
                    pathStack.Push(pathTile);
                    pathTile = pathTile.prevPathTileNodeMap[_cCharacter.name];
                }
                return;
            }
            else
            {
                for (int i = 0; i < (int)ChessCharacter.Direction.MAXSIZE; i++)
                {
                    ChessCharacter.Direction direction = (ChessCharacter.Direction)i;
                    Vector3Int nextTilePos = currentTile.position + _cCharacter.GetDirectionTileNext(direction);
                    ChessTile nextTile = GameManager.gameInstance.tilemap.GetTile<ChessTile>(nextTilePos);
                    if (_cCharacter.CanMoveTile(nextTilePos) && nextTile.prevPathTileNodeMap[_cCharacter.name] == null)
                    {
                        nextTile.prevPathTileNodeMap[_cCharacter.name] = currentTile;
                        //Debug.Log("nextTile : " + nextTile.position + "direction : " + direction);
                        findQueue.Enqueue(nextTile);
                    }
                }
            }
        }
    }
}
