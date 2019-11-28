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

    public int _findRange = 15;
    public int _attackRange = 1;
    private float _hp;

    private ChessCharacter _moveTarget;
    private ChessCharacter _attackTarget;

    protected Vector3Int tilePosition;

    public enum eCharacterBattleState { BATTLE, WAIT, MAXSIZE };
    protected eCharacterBattleState _battleState;

    public enum eState { IDLE, MOVE, ATTACK, DEAD, MAXSIZE };
    public eState _state;
    protected eState _prevState;

    protected Dictionary<eState, State> stateMap;

    public enum Direction { UP, DOWN, LEFT, RIGHT, MAXSIZE };
    protected Direction _direction;

    public enum eCharacterType { PLAYER, ENEMY, WAIT, MAXSIZE };
    public eCharacterType _characterType;

    protected Animator animator;

    protected Tilemap tilemap;

    private Queue<ChessTile> _pathFindQueue;
    private Stack<ChessTile> _pathStack;

    public void ClearPathFindQueue()
    {
        _pathFindQueue.Clear();
    }

    public void PushPathFindTile(ChessTile chessTile)
    {
        _pathFindQueue.Enqueue(chessTile);
    }

    public ChessTile PopPathFindTile()
    {
        return _pathFindQueue.Dequeue();
    }

    public void ClearPathStack()
    {
        _pathStack.Clear();
    }

    public ChessTile PopPathStackTile()
    {
        return _pathStack.Pop();
    }

    public void PushPathStackTile(ChessTile chessTile)
    {
        _pathStack.Push(chessTile);
    }

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
        _battleState = eCharacterBattleState.BATTLE;
        Debug.Log("InitData in ChessCharacter");

        _direction = Direction.RIGHT;
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        transform.position = tilemap.layoutGrid.CellToWorld(tilePosition);

        _pathFindQueue = new Queue<ChessTile>();
        _pathStack = new Stack<ChessTile>();

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
        stateMap = new Dictionary<eState, State>();
        
        stateMap[eState.IDLE] = new EnemyFindIdleState();
        stateMap[eState.MOVE] = new PlayerMoveState();
        stateMap[eState.ATTACK] = new AttackState();
        stateMap[eState.DEAD] = new DeadState();

        for (eState i = 0; i < eState.MAXSIZE; i++)
        {
            stateMap[i].InitState(this);
        }

        SetState(eState.IDLE);
        _prevState = eState.IDLE;
        //초기 세팅만 startState 호출(이후 변경으로 인한 내용은 update에서 감지
        stateMap[_state].StartState();
        _prevState = _state;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.gameInstance._round == GameManager.eRound.BATTLE)
        {
            if (_state != _prevState)
            {
                //Debug.Log(this.name + " : end prev state: " + _prevState);
                //Debug.Log(this.name + " : start cur state : " + _state);
                stateMap[_prevState].EndState();
                stateMap[_state].StartState();
                _prevState = _state;
            }
            stateMap[_state].UpdateState();
        }

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

    private GameManager.eCharacter eChessCharacterType;
    public GameManager.eCharacter GetChessCharacterType()
    {
        return eChessCharacterType;
    }

    public void SetChessCharacterType(GameManager.eCharacter character)
    {
        eChessCharacterType = character;
    }

    public void SetCharacterType(eCharacterType characterType)
    {
        _characterType = characterType;
    }

    public eCharacterType GetCharacterType()
    {
        return _characterType;
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

    public void SetMoveTarget(ChessCharacter moveTarget)
    {
        _moveTarget = moveTarget;
    }

    public ChessCharacter GetMoveTarget()
    {
        return _moveTarget;
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
            Debug.Log("AnimateEvent : AttackDamage!!!!!!");
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
            Debug.Log("AnimateEvent : AttackEnd!!!!!!");
            animator.SetBool("isAttack", false);
            SetState(eState.IDLE);
        }
        if (message.Equals("DeadEnd"))
        {
            Debug.Log("AnimateEvent : DeadEnd!!!!!!");
            gameObject.SetActive(false);
            _hpBar.gameObject.SetActive(false);
            Destroy(gameObject);
            Destroy(_hpBar);
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

    public Slider GetHpBar()
    {
        return _hpBar;
    }
}
