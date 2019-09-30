using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : MonoBehaviour
{
    public float moveSpeed = 1.0f;
    private Vector3 targetPosition;
    private Vector3 moveTotalVector;
    private bool isMoving;

	private enum Direction { LEFT, RIGHT };
	private Direction direction;

    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        targetPosition = transform.position;
        isMoving = false;
        animator = GetComponent<Animator>();
        animator.SetBool("isMoving", false);

		direction = Direction.RIGHT;
		transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
	}

    // Update is called once per frame
    void Update()
    {
        if(!isMoving)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                targetPosition = transform.position + new Vector3(-1.0f, 0.5f, 0.0f);
                isMoving = true;
                animator.SetBool("isMoving", true);
                moveTotalVector = targetPosition - transform.position;
				direction = Direction.LEFT;
				transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
			}
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                targetPosition = transform.position + new Vector3(1.0f, -0.5f, 0.0f);
                isMoving = true;
                animator.SetBool("isMoving", true);
                moveTotalVector = targetPosition - transform.position;
				direction = Direction.RIGHT;
				transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
			}
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                targetPosition = transform.position + new Vector3(-1.0f, -0.5f, 0.0f);
                isMoving = true;
                animator.SetBool("isMoving", true);
                moveTotalVector = targetPosition - transform.position;
				direction = Direction.LEFT;
				transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
			}
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                targetPosition = transform.position + new Vector3(1.0f, 0.5f, 0.0f);
                isMoving = true;
                animator.SetBool("isMoving", true);
                moveTotalVector = targetPosition - transform.position;
				direction = Direction.RIGHT;
				transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
			}
        }

        if (isMoving)
        {
            transform.position = transform.position + moveTotalVector * (moveSpeed * Time.deltaTime);
            Debug.Log("transform.position : " + transform.position + "targetPosition : " + targetPosition);
            Debug.Log(Vector3.Distance(transform.position, targetPosition));
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                transform.position = targetPosition;
                isMoving = false;
                animator.SetBool("isMoving", false);
            }
        }
    }
}
