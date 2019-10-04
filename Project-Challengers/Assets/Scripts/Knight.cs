using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Knight : MonoBehaviour
{
    public float moveSpeed = 1.0f;
    private Vector3 targetPosition;
    private Vector3 moveTotalVector;
    private bool isMoving;

    private Vector3Int tilePosition;

	private enum Direction { LEFT, RIGHT };
	private Direction direction;

    private Animator animator;

    private Tilemap tilemap;

	private bool canInput = false;
	// Start is called before the first frame update
	void Start()
    {
		Debug.Log("Start Knight");
        targetPosition = transform.position;
        isMoving = false;
        animator = GetComponent<Animator>();
        animator.SetBool("isMoving", false);

		direction = Direction.RIGHT;
		transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

		tilemap = GameManager.gameInstance.tilemap;
		transform.position = tilemap.layoutGrid.CellToWorld(tilePosition);
		Debug.Log("Start Position tileX : " + tilePosition.x + " | tileY : " + tilePosition.y);
	}

    // Update is called once per frame
    void Update()
    {
        if(!isMoving)
        {
            if (canInput)
			{
                Vector3Int currentTilePos = tilePosition;
                if (Input.GetKeyDown(KeyCode.UpArrow))
				{
                    tilePosition.y++;
                    if (!CanMoveTile()) //움직일 수 없을 때 원래 위치로 되돌린다.
                    {
                        tilePosition = currentTilePos;
                    }

                    //Vector3Int currentTilePos = tilePosition;
                    //currentTilePos.y++;
                    //if (!CanMoveTile())
                    //{
                    //    tilePosition = currentTilePos;
                    //}
					Move();
					SetDirection(Direction.LEFT);
				}
				if (Input.GetKeyDown(KeyCode.DownArrow))
				{
                    tilePosition.y--;
                    if (!CanMoveTile())
                    {
                        tilePosition = currentTilePos;
                    }
                    Move();
					SetDirection(Direction.RIGHT);
				}
				if (Input.GetKeyDown(KeyCode.LeftArrow))
				{
                    tilePosition.x--;
                    if (!CanMoveTile())
                    {
                        tilePosition = currentTilePos;
                    }
                    Move();
					SetDirection(Direction.LEFT);
				}
				if (Input.GetKeyDown(KeyCode.RightArrow))
				{
                    tilePosition.x++;
                    if (!CanMoveTile())
                    {
                        tilePosition = currentTilePos;
                    }
                    Move();
					SetDirection(Direction.RIGHT);
				}
			}
        }

        if (isMoving)
        {
            transform.position = transform.position + moveTotalVector * (moveSpeed * Time.deltaTime);
            //Debug.Log(Vector3.Distance(transform.position, targetPosition));
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                transform.position = targetPosition;
                isMoving = false;
                animator.SetBool("isMoving", false);
            }
        }
    }

    private void Move()
    {
        targetPosition = tilemap.layoutGrid.CellToWorld(tilePosition);
        isMoving = true;
        animator.SetBool("isMoving", true);
        moveTotalVector = targetPosition - transform.position;
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
    }

    private bool CanMoveTile()
    {
        if (tilePosition.x < 1 || tilePosition.x > 8
         || tilePosition.y < 1 || tilePosition.y > 8)
        {
            return false;
        }
        return true;
    }
}
