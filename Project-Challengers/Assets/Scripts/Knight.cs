using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : MonoBehaviour
{
    public float moveSpeed = 1.0f;
    private Vector3 targetPosition;
    private Vector3 moveTotalVector;
    private bool isMoving;
    // Start is called before the first frame update
    void Start()
    {
        targetPosition = transform.position;
        isMoving = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isMoving)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                targetPosition = transform.position + new Vector3(0.0f, 1.0f, 0.0f);
                isMoving = true;
                moveTotalVector = targetPosition - transform.position;
                //transform.position = transform.position + new Vector3(0.0f, 1.0f, 0.0f);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                targetPosition = transform.position + new Vector3(0.0f, -1.0f, 0.0f);
                isMoving = true;
                moveTotalVector = targetPosition - transform.position;
                //transform.position = transform.position + new Vector3(0.0f, -1.0f, 0.0f);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                targetPosition = transform.position + new Vector3(-1.0f, 0.0f, 0.0f);
                isMoving = true;
                moveTotalVector = targetPosition - transform.position;
                //transform.position = transform.position + new Vector3(-1.0f, 0.0f, 0.0f);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                targetPosition = transform.position + new Vector3(1.0f, 0.0f, 0.0f);
                isMoving = true;
                moveTotalVector = targetPosition - transform.position;
                //transform.position = transform.position + new Vector3(1.0f, 0.0f, 0.0f);
            }
        }

        if (isMoving)
        {
             transform.position = transform.position + moveTotalVector * (moveSpeed * Time.deltaTime);
            Debug.Log("transform.position : " + transform.position + "targetPosition : " + targetPosition);
            Debug.Log(Vector3.Distance(transform.position, targetPosition));
            if (Vector3.Distance(transform.position, targetPosition) < 0.02f)
            {
                transform.position = targetPosition;
                isMoving = false;
            }
        }
    }
}
