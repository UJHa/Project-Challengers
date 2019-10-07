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
        GameManager.gameInstance.SetTileData(tilePosition, 1);
        Debug.Log("Start Position tileX : " + tilePosition.x + " | tileY : " + tilePosition.y);
	}

    // Update is called once per frame
    void Update()
    {
        if(!isMoving)
        {
            if (canInput)
			{
                Vector3Int prevTilePos = tilePosition;
                Vector3Int nextTilePos = tilePosition;
                if (Input.GetKeyDown(KeyCode.UpArrow))
				{
                    nextTilePos.y++;
                    if (CanMoveTile(nextTilePos))
                    {
                        SetTilePosition(nextTilePos);
                    }
                    Move();
					SetDirection(Direction.LEFT);
				}
				if (Input.GetKeyDown(KeyCode.DownArrow))
				{
                    nextTilePos.y--;
                    if (CanMoveTile(nextTilePos))
                    {
                        SetTilePosition(nextTilePos);
                    }
                    Move();
					SetDirection(Direction.RIGHT);
				}
				if (Input.GetKeyDown(KeyCode.LeftArrow))
				{
                    nextTilePos.x--;
                    if (CanMoveTile(nextTilePos))
                    {
                        SetTilePosition(nextTilePos);
                    }
                    Move();
					SetDirection(Direction.LEFT);
				}
				if (Input.GetKeyDown(KeyCode.RightArrow))
				{
                    nextTilePos.x++;
                    if (CanMoveTile(nextTilePos))
                    {
                        SetTilePosition(nextTilePos);
                    }
                    Move();
					SetDirection(Direction.RIGHT);
				}
                GameManager.gameInstance.SetTileData(prevTilePos, 0);
                GameManager.gameInstance.SetTileData(tilePosition, 1);
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
}
