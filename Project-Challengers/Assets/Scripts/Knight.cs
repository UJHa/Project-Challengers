using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Knight : MonoBehaviour
{
    public GameObject weapon;
    private GameObject weaponObject;

    public float moveSpeed = 1.0f;

    private Vector3 targetPosition;
    private Vector3 moveTotalVector;

    private Vector3Int tilePosition;

    private enum eState { IDLE, MOVE, ATTACK, MaxSize };
    private eState _state;

    private Dictionary<eState, State> stateMap;

	public enum Direction { LEFT, RIGHT };
	private Direction direction;

    private Animator animator;

    private Tilemap tilemap;

	private bool canInput = false;

    // Start is called before the first frame update
    void Start()
    {
		Debug.Log("Start Knight");

        // 데이터들 초기 세팅
        targetPosition = transform.position;
        animator = GetComponent<Animator>();
        animator.SetBool("isMoving", false);

        direction = Direction.RIGHT;
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

        stateMap = new Dictionary<eState, State>();
        stateMap[eState.IDLE] = new IdleState();
        stateMap[eState.MOVE] = new MoveState();
        stateMap[eState.ATTACK] = new AttackState();

        for (eState i = 0; i < eState.MaxSize; i++)
        {
            stateMap[i].SetCharacter(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        {
            stateMap[_state].UpdateState();
        }
        //// 대기 상태
        //if(eState.IDLE == state)
        //{
        //    IdleUpdate();
        //}
        //
        //// 이동 상태
        //if(eState.MOVE == state)
        //{
        //    MoveUpdate();
        //}
        //
        ////공격 상태
        //if (eState.ATTACK == state)
        //{
        //    AttackUpdate();
        //}
    }

    private void MoveStart()
    {
        targetPosition = tilemap.layoutGrid.CellToWorld(tilePosition);
        animator.SetBool("isMoving", true);
        moveTotalVector = targetPosition - transform.position;
        SetState(eState.MOVE);
    }

    private void SetDirection(Direction direct)
    {
        direction = direct;
        switch (direction)
        {
            case Direction.LEFT:
                transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                break;
            case Direction.RIGHT:
                transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                break;
            default:
                break;
        }
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

    private bool CanMoveTile(Vector3Int tilePos)
    {
        if (tilePos.x < 1 || tilePos.x > 8
         || tilePos.y < 1 || tilePos.y > 8)
        {
            return false;
        }
        if (GameManager.gameInstance.GetTileData(tilePos) == 1)
        {
            return false;
        }
        return true;
    }

    public void MoveRequest(Vector3Int movePos, Direction direction)
    {
        if (eState.IDLE != _state) return;

        SetDirection(direction);

        Vector3Int nextTilePos = tilePosition + movePos;

        if (CanMoveTile(nextTilePos))
        {
            GameManager.gameInstance.SetTileData(tilePosition, 0);
            SetTilePosition(nextTilePos);
            MoveStart();
        }
    }

    private void SetState(eState state)
    {
        _state = state;
    }

    public void IdleUpdate()
    {
        if (canInput)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                MoveRequest(Vector3Int.up, Direction.LEFT);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveRequest(Vector3Int.down, Direction.RIGHT);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveRequest(Vector3Int.left, Direction.LEFT);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveRequest(Vector3Int.right, Direction.RIGHT);
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("space!!!");
                weaponObject.transform.localEulerAngles = Vector3.zero;
                SetState(eState.ATTACK);
            }
        }
    }

    public void MoveUpdate()
    {
        transform.position = transform.position + moveTotalVector * (moveSpeed * Time.deltaTime);
        //Debug.Log(Vector3.Distance(transform.position, targetPosition));
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            transform.position = targetPosition;
            animator.SetBool("isMoving", false);
            SetState(eState.IDLE);
        }
    }

    public void AttackUpdate()
    {
        weaponObject.transform.localEulerAngles = Vector3.Lerp(weaponObject.transform.localEulerAngles, new Vector3(0, 0, -90), Time.deltaTime * 1.0f);
        Debug.Log("attacking log : " + weaponObject.transform.localEulerAngles.z);
        if (weaponObject.transform.localEulerAngles.z < 260)
        {
            weaponObject.transform.localEulerAngles = weapon.transform.eulerAngles;
            SetState(eState.IDLE);
        }
    }
}
