﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChessCharacter : MonoBehaviour
{
    public GameObject weapon;
    protected GameObject weaponObject;

    public float moveSpeed = 1.0f;

    protected Vector3 targetPosition;

    protected Vector3Int tilePosition;

    public enum eState { IDLE, MOVE, ATTACK, MAXSIZE };
    protected eState _state;
    protected eState _currentState;

    protected Dictionary<eState, State> stateMap;

    public enum Direction { UP, DOWN, LEFT, RIGHT };
    protected Direction _direction;

    protected Animator animator;

    protected Tilemap tilemap;

    protected bool canInput = false;

    // Start is called before the first frame update
    void Start()
    {
        // 데이터들 초기 세팅
        targetPosition = transform.position;
        animator = GetComponent<Animator>();
        animator.SetBool("isMoving", false);

        _direction = Direction.RIGHT;
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        tilemap = GameManager.gameInstance.tilemap;
        transform.position = tilemap.layoutGrid.CellToWorld(tilePosition);
        Debug.Log("Start Position tileX : " + tilePosition.x + " | tileY : " + tilePosition.y);
        GameManager.gameInstance.SetTileData(tilePosition, 1);

        // 무기 초기 세팅
        weaponObject = Instantiate(weapon) as GameObject;
        weaponObject.transform.parent = transform;
        weaponObject.transform.localPosition = weaponObject.transform.position;

        SetState(eState.IDLE);
        _currentState = eState.IDLE;

        stateMap = new Dictionary<eState, State>();
        if (canInput)
        {
            stateMap[eState.IDLE] = new PlayerIdleState();
        }
        else
        {
            stateMap[eState.IDLE] = new IdleState();
        }
        
        stateMap[eState.MOVE] = new MoveState();
        stateMap[eState.ATTACK] = new AttackState();

        for (eState i = 0; i < eState.MAXSIZE; i++)
        {
            stateMap[i].InitState(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_state != _currentState)
        {
            stateMap[_state].StartState();
            _currentState = _state;
        }
        stateMap[_state].UpdateState();
        if (_state != _currentState)
        {
            stateMap[_state].EndState();
        }
    }

    public void MoveStart()
    {
        targetPosition = tilemap.layoutGrid.CellToWorld(tilePosition);
        animator.SetBool("isMoving", true);
        SetState(eState.MOVE);
    }

    public void SetDirection(Direction direct)
    {
        _direction = direct;
        switch (_direction)
        {
            case Direction.UP:
            case Direction.LEFT:
                transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                break;
            case Direction.DOWN:
            case Direction.RIGHT:
                transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                break;
            default:
                break;
        }
    }

    public Direction GetDirection()
    {
        return _direction;
    }

    public Vector3Int GetDirectionTileNext(Direction direction)
    {
        Vector3Int result = Vector3Int.zero;
        switch (direction)
        {
            case Direction.UP:
                result = Vector3Int.up;
                break;
            case Direction.DOWN:
                result = Vector3Int.down;
                break;
            case Direction.LEFT:
                result = Vector3Int.left;
                break;
            case Direction.RIGHT:
                result = Vector3Int.right;
                break;
            default:
                break;
        }
        return result;
    }

    public void IsPlayer(bool isPlayer)
    {
        canInput = isPlayer;
    }

    public Vector3Int GetTilePosition()
    {
        return tilePosition;
    }

    public void SetTilePosition(Vector3Int tilePos)
    {
        tilePosition = tilePos;
        GameManager.gameInstance.SetTileData(tilePosition, 1);
    }

    public bool CanMoveTile(Direction direction)
    {
        Vector3Int tilePos = tilePosition + GetDirectionTileNext(direction);
        Debug.Log("GameManager.gameInstance.tileMap data : " + GameManager.gameInstance.tilemap.GetColliderType(new Vector3Int(tilePos.x, tilePos.y, 0)));
        if (tilePos.x < 0 || tilePos.x > 7
         || tilePos.y < 0 || tilePos.y > 7)
        {
            return false;
        }
        if (GameManager.gameInstance.GetTileData(tilePos) == 1)
        {
            return false;
        }
        return true;
    }

    public void MoveRequest(Direction direction)
    {
        if (eState.IDLE != _state) return;

        SetDirection(direction);
        if (CanMoveTile(direction))
        {
            GameManager.gameInstance.SetTileData(tilePosition, 0);
            SetTilePosition(tilePosition + GetDirectionTileNext(direction));
            MoveStart();
        }
    }

    public void SetState(eState state)
    {
        _state = state;
    }

    public eState GetState()
    {
        return _state;
    }

    public void MoveUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, targetPosition) == 0.0f)
        {
            MoveFinish();
        }
    }

    private void MoveFinish()
    {
        animator.SetBool("isMoving", false);
        SetState(eState.IDLE);
    }

    public void AttackStart()
    {
        weaponObject.transform.localEulerAngles = Vector3.zero;
    }

    public void AttackUpdate()
    {
        weaponObject.transform.localEulerAngles = Vector3.Slerp(weaponObject.transform.localEulerAngles, new Vector3(0, 0, -90), Time.deltaTime * 1.0f);
        //Debug.Log("attacking log : " + weaponObject.transform.localEulerAngles.z);
        if (weaponObject.transform.localEulerAngles.z < 260)
        {
            weaponObject.transform.localEulerAngles = weapon.transform.eulerAngles;
            SetState(eState.IDLE);
        }
    }
}