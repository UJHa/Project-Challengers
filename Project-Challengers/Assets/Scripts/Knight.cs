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

    private int tileX;
    private int tileY;

	private enum Direction { LEFT, RIGHT };
	private Direction direction;

    private Animator animator;

    private Tilemap tilemap;
    // Start is called before the first frame update
    void Start()
    {
        targetPosition = transform.position;
        isMoving = false;
        animator = GetComponent<Animator>();
        animator.SetBool("isMoving", false);

		direction = Direction.RIGHT;
		transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        tilemap = GameManager.gameInstance.tilemap;
        tileX = tilemap.layoutGrid.WorldToCell(transform.position).x;
        tileY = tilemap.layoutGrid.WorldToCell(transform.position).y;
        Debug.Log("tileX : " + tileX);
        Debug.Log("tileY : " + tileY);
    }

    // Update is called once per frame
    void Update()
    {
        if(!isMoving)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                tileY++;
                Move();
                SetDirection(Direction.LEFT);
			}
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                tileY--;
                Move();
                SetDirection(Direction.RIGHT);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                tileX--;
                Move();
                SetDirection(Direction.LEFT);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                tileX++;
                Move();
                SetDirection(Direction.RIGHT);
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
        targetPosition = tilemap.layoutGrid.CellToWorld(new Vector3Int(tileX, tileY, 0));
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
}
