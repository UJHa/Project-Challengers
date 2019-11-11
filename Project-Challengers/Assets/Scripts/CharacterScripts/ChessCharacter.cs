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

    public int _findRange = 3;
    public int _attackRange = 1;
    private float _hp;

    protected Vector3 targetPosition;
    protected Vector3Int mouseTargetTilePosition;

    protected Vector3Int tilePosition;

    public enum eCharacterBattleState { BATTLE, WAIT, MAXSIZE };
    protected eCharacterBattleState _battleState;

    public enum eState { IDLE, MOVE, ATTACK, DEAD, MAXSIZE };
    protected eState _state;
    protected eState _prevState;

    protected Dictionary<eState, State> stateMap;

    public enum Direction { UP, DOWN, LEFT, RIGHT, MAXSIZE };
    protected Direction _direction;

    public enum eCharacterType { PLAYER, ENEMY, MAXSIZE };
    public eCharacterType _characterType;

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
        InitData();
        InitState();
    }

    protected virtual void InitData()
    {
        // 데이터들 초기 세팅
        targetPosition = transform.position;
        _battleState = eCharacterBattleState.BATTLE;
        Debug.Log("InitData in ChessCharacter");

        _direction = Direction.RIGHT;
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        transform.position = tilemap.layoutGrid.CellToWorld(tilePosition);
        mouseTargetTilePosition = tilePosition;
        _attackTarget = null;

        Debug.Log(this.name + " : Start Position tileX : " + tilePosition.x + " | tileY : " + tilePosition.y);
        tilemap.SetColliderType(tilePosition, Tile.ColliderType.Grid);
        _hp = maxHp;
        animator = GetComponent<Animator>();
        animator.SetBool("isMoving", false);
        animator.SetBool("isDead", false);
    }

    protected virtual void InitState()
    {
        SetState(eState.IDLE);
        _prevState = eState.IDLE;

        stateMap = new Dictionary<eState, State>();
        if (canInput)
        {
            stateMap[eState.IDLE] = new PlayerIdleState();
            stateMap[eState.MOVE] = new PlayerMoveState();
        }
        else
        {
            stateMap[eState.IDLE] = new EnemyFindIdleState();
            stateMap[eState.MOVE] = new PlayerMoveState();
        }
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
        if (_state != _prevState)
        {
            //Debug.Log(this.name + " : end prev state: " + _prevState);
            //Debug.Log(this.name + " : start cur state : " + _state);
            stateMap[_prevState].EndState();
            stateMap[_state].StartState();
            _prevState = _state;
        }
        stateMap[_state].UpdateState();
        

        //UI update
        if(_battleState == eCharacterBattleState.BATTLE)
        {
            GameObject canvas = GameObject.Find("Canvas");
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            Vector3 characterUiPos = Camera.main.WorldToScreenPoint(transform.position) + new Vector3(canvasRect.rect.x, canvasRect.rect.y, 0);
            characterUiPos.y += 100;
            _hpBar.gameObject.transform.localPosition = characterUiPos;
            _hpBar.value = _hp / maxHp;
            switch (_characterType)
            {
                case eCharacterType.PLAYER:
                    _hpBar.fillRect.gameObject.GetComponent<Image>().color = Color.red;
                    Debug.Log("PLS none!!!");
                    break;
                case eCharacterType.ENEMY:
                    _hpBar.fillRect.gameObject.GetComponent<Image>().color = new Color(155.0f / 255.0f, 0.0f, 0.0f, 1.0f);
                    break;
            }
        }
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

    public void SetCharacterType(eCharacterType characterType)
    {
        _characterType = characterType;
    }

    public eCharacterType GetCharacterType()
    {
        return _characterType;
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

    public bool IsInWall(Vector3Int tilePos)
    {
        if (tilePos.x < 0 || tilePos.x > 7
         || tilePos.y < 0 || tilePos.y > 7)
        {
            return false;
        }
        return true;
    }

    public bool CanMoveTile(Vector3Int tilePos)
    {
        if (tilePos.x < 0 || tilePos.x > 7
         || tilePos.y < 0 || tilePos.y > 7)
        {
            return false;
        }

        if ((int)tilemap.GetColliderType(tilePos) > (int)Tile.ColliderType.None)
        {
            return false;
        }
        ChessTile chessTile = tilemap.GetTile<ChessTile>(tilePos);
        if (chessTile.gameObject != null)
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
        Vector3Int nextTilePos = tilePosition + GetDirectionTileNext(direction);
        if (CanMoveTile(nextTilePos))
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

    public void MoveStart()
    {
        targetPosition = tilemap.layoutGrid.CellToWorld(tilePosition);
        SetState(eState.MOVE);
    }

    public void MouseMoveStart()
    {
        mouseTargetTilePosition = tilemap.layoutGrid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        //mouseTargetTilePosition의 x,y가 -1씩 되어 있다 원인은 파악하지 못해서 임시 처리
        mouseTargetTilePosition.x += 1;
        mouseTargetTilePosition.y += 1;
        mouseTargetTilePosition.z = 0;

        //Debug.Log("name : " + this.name);
        //Debug.Log("Input mouse!2 : " + tilemap.transform.position);
        SetState(eState.MOVE);
    }

    public Vector3 GetTargetPosition()
    {
        return targetPosition;
    }

    public void SetTargetTilePosition(Vector3Int targetTilePosition)
    {
        mouseTargetTilePosition = targetTilePosition;
    }

    public Vector3Int GetTargetTilePosition()
    {
        return mouseTargetTilePosition;
    }

    public void SetState(eState state)
    {
        _state = state;
    }

    public eState GetState()
    {
        return _state;
    }

    public virtual void AttackStart() { }

    public virtual void AttackUpdate() { }

    protected virtual void AttackDamage(int damage)
    {
        Debug.Log("name : " + name);
        Debug.Log("데미지 : " + damage);
        _hp -= damage;
        if (_hp <= 0)
        {
            animator.SetBool("isDead", true);
            _hp = 0;
            SetState(eState.DEAD);
            tilemap.SetColliderType(tilePosition, Tile.ColliderType.None);
            GameManager.gameInstance.SetTileObject(tilePosition, null);
            
        }
    }

    private ChessCharacter _attackTarget;
    public void SetAttackTarget(ChessCharacter tileCharacter)
    {
        _attackTarget = tileCharacter;
    }
    public ChessCharacter GetAttackTarget()
    {
        return _attackTarget;
    }

    public void AnimateEvent(string message)
    {
        if (message.Equals("AttackDamage")) // AttackDamage 함수와 헷깔리므로 이름 변경해야 합니다.
        {
            Debug.Log("TEST3!!!!!!");
            if (_attackTarget != null)
            {
                GameObject gameObject = GameManager.gameInstance.GetTileObject(_attackTarget.GetTilePosition());
                if (gameObject != null)
                {
                    ChessCharacter character = gameObject.GetComponent<ChessCharacter>();
                    Debug.Log("character : " + character);
                    character.SendMessage("AttackDamage", GetAttackPower());
                }
            }
        }
        if (message.Equals("AttackEnd"))
        {
            Debug.Log("TEST!!!!!!");
            animator.SetBool("isAttack", false);
            SetState(eState.IDLE);
        }
        if (message.Equals("DeadEnd"))
        {
            Debug.Log("TEST2!!!!!!");
            gameObject.SetActive(false);
            _hpBar.gameObject.SetActive(false);
        }
    }

    public int GetAttackPower()
    {
        return _attackPower;
    }

    //Animation
    public Animator GetAnimator()
    {
        return animator;
    }

    //UI
    private Slider _hpBar;

    public void SetHpBar(Slider slider)
    {
        _hpBar = slider;
    }
}
