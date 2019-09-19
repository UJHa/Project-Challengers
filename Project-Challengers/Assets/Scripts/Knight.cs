﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            transform.position = transform.position + new Vector3(0.0f, 1.0f, 0.0f);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            transform.position = transform.position + new Vector3(0.0f, -1.0f, 0.0f);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.position = transform.position + new Vector3(-1.0f, 0.0f, 0.0f);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.position = transform.position + new Vector3(1.0f, 0.0f, 0.0f);
        }
    }
}
