using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class ChessCharacter : MonoBehaviour
{

    public float moveSpeed = 1.0f;
    public float maxHp = 300.0f;
    public int _attackPower = 10;
    private float _hp;

    protected Vector3 targetPosition;

    protected Vector3Int tilePosition;

    public enum eState { IDLE, MOVE, ATTACK, DEAD, MAXSIZE };
    protected eState _state;
    protected eState _currentState;

    protected Dictionary<eState, State> stateMap;

    public enum Direction { UP, DOWN, LEFT, RIGHT };
    protected Direction _direction;

    protected Animator animator;

    protected Tilemap tilemap;

    protected bool canInput = false;

    void Awake()
    {
        tilemap = GameManager.gameInstance.tilemap;
    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start()");
        InitData();
        InitState();
    }

    protected virtual void InitData()
    {
        // 데이터들 초기 세팅
        targetPosition = transform.position;

        _direction = Direction.RIGHT;
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        transform.position = tilemap.layoutGrid.CellToWorld(tilePosition);
        Debug.Log("Start Position tileX : " + tilePosition.x + " | tileY : " + tilePosition.y);
        tilemap.SetColliderType(tilePosition, Tile.ColliderType.Grid);

        _hp = maxHp;

        animator = GetComponent<Animator>();
        animator.SetBool("isMoving", false);
        animator.SetBool("isDead", false);
    }

    protected virtual void InitState()
    {
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
        stateMap[eState.DEAD] = new DeadState();

        for (eState i = 0; i < eState.MAXSIZE; i++)
        {
            stateMap[i].InitState(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //State update
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

        //UI update
        {
            GameObject canvas = GameObject.Find("Canvas");
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            Vector3 characterUiPos = Camera.main.WorldToScreenPoint(transform.position) + new Vector3(canvasRect.rect.x, canvasRect.rect.y, 0);
            characterUiPos.y += 50;
            _hpBar.gameObject.transform.localPosition = characterUiPos;
            _hpBar.value = _hp / maxHp;
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
        tilemap.SetColliderType(tilePosition, Tile.ColliderType.Grid);
        GameManager.gameInstance.SetTileObject(tilePosition, gameObject);
    }

    public bool CanMoveTile(Direction direction)
    {
        Vector3Int tilePos = tilePosition + GetDirectionTileNext(direction);
        Debug.Log("tilePosition : " + tilePosition);
        Debug.Log("tilePos : " + tilePos);
        //Debug.Log("GameManager.gameInstance.tileMap data : " + GameManager.gameInstance.tilemap.GetColliderType(new Vector3Int(tilePos.x, tilePos.y, 0)));
        if (tilePos.x < 0 || tilePos.x > 7
         || tilePos.y < 0 || tilePos.y > 7)
        {
            return false;
        }
        if ((int)tilemap.GetColliderType(tilePos) > (int)Tile.ColliderType.None)
        {
            return false;
        }
        return true;
    }

    public void MoveRequest(Direction direction)
    {
        if (eState.IDLE != _state) return;

        Debug.Log("direction : " + direction);
        SetDirection(direction);
        if (CanMoveTile(direction))
        {
            Debug.Log(name + " : 이동 성공");
            tilemap.SetColliderType(tilePosition, Tile.ColliderType.None);
            GameManager.gameInstance.SetTileObject(tilePosition, null);
            SetTilePosition(tilePosition + GetDirectionTileNext(direction));
            MoveStart();
        }
        else
        {
            Debug.Log(name + " : 이동 실패");
            SetState(eState.ATTACK);
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

    public virtual void AttackStart()
    {
        
    }

    public virtual void AttackUpdate()
    {
        
    }

    private void AttackDamage(int damage)
    {
        Debug.Log("name : " + name);
        Debug.Log("데미지 : " + damage);
        _hp -= damage;
        if (_hp <= 0)
        {
            animator.SetBool("isDead", true);
            _hp = 0;
            SetState(eState.DEAD);
            gameObject.SetActive(false);
            _hpBar.gameObject.SetActive(false);
            tilemap.SetColliderType(tilePosition, Tile.ColliderType.None);
        }
    }

    public int GetAttackPower()
    {
        return _attackPower;
    }

    //UI
    private Slider _hpBar;

    public void SetHpBar(Slider slider)
    {
        _hpBar = slider;
    }
}
