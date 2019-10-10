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

    private enum eState { IDLE, MOVE, ATTACK };
    private eState state;

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

        state = eState.IDLE;
    }

    // Update is called once per frame
    void Update()
    {
        //if(!isMoving)   // 대기 상태
        if(eState.IDLE == state)
        {
            if (canInput)
			{
                Vector3Int movePosition = Vector3Int.zero;
                if (Input.GetKeyDown(KeyCode.UpArrow))
				{
                    movePosition.y++;
                    MoveRequest(movePosition, Direction.LEFT);
                }
				if (Input.GetKeyDown(KeyCode.DownArrow))
				{
                    movePosition.y--;
                    MoveRequest(movePosition, Direction.RIGHT);
                }
				if (Input.GetKeyDown(KeyCode.LeftArrow))
				{
                    movePosition.x--;
                    MoveRequest(movePosition, Direction.LEFT);
				}
				if (Input.GetKeyDown(KeyCode.RightArrow))
				{
                    movePosition.x++;
                    MoveRequest(movePosition, Direction.RIGHT);
				}
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Debug.Log("space!!!");
                    weaponObject.transform.localEulerAngles = Vector3.zero;
                    state = eState.ATTACK;
                }
            }
        }

        //if (isMoving)   // 이동 상태
        if(eState.MOVE == state)
        {
            transform.position = transform.position + moveTotalVector * (moveSpeed * Time.deltaTime);
            //Debug.Log(Vector3.Distance(transform.position, targetPosition));
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                transform.position = targetPosition;
                animator.SetBool("isMoving", false);
                state = eState.IDLE;
            }
        }

        //공격 상태
        if (eState.ATTACK == state)
        {
            weaponObject.transform.localEulerAngles = Vector3.Lerp(weaponObject.transform.localEulerAngles, new Vector3(0, 0, -90), Time.deltaTime * 1.0f);
            Debug.Log("attacking log : " + weaponObject.transform.localEulerAngles.z);
            if (weaponObject.transform.localEulerAngles.z < 260)
            {
                weaponObject.transform.localEulerAngles = weapon.transform.eulerAngles;
                state = eState.IDLE;
            }
        }
    }

    private void MoveStart()
    {
        targetPosition = tilemap.layoutGrid.CellToWorld(tilePosition);
        animator.SetBool("isMoving", true);
        moveTotalVector = targetPosition - transform.position;
        state = eState.MOVE;
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
        if (eState.IDLE != state) return;

        SetDirection(direction);

        Vector3Int nextTilePos = tilePosition + movePos;

        if (CanMoveTile(nextTilePos))
        {
            GameManager.gameInstance.SetTileData(tilePosition, 0);
            SetTilePosition(nextTilePos);
            MoveStart();
        }
    }
}
