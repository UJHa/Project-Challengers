using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempPlayer : MonoBehaviour
{
    public GameObject coin;
    // Start is called before the first frame update
    void Start()
    {
		Debug.Log("Hello world!");
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.A))
		{
			Vector3 vec = transform.position;
			Debug.Log("Move Left!");
			vec.x -= 1.0f;
			transform.position = vec;
		}
		if (Input.GetKeyDown(KeyCode.D))
		{
			Vector3 vec = transform.position;
			Debug.Log("Move Right!");
			vec.x += 1.0f;
			transform.position = vec;
		}
		if (Input.GetKeyDown(KeyCode.W))
		{
			Vector3 vec = transform.position;
			Debug.Log("Move Up!");
			vec.y += 1.0f;
			transform.position = vec;
		}
		if (Input.GetKeyDown(KeyCode.S))
		{
			Vector3 vec = transform.position;
			Debug.Log("Move Down!");
			vec.y -= 1.0f;
			transform.position = vec;
		}
        if (Input.GetKeyDown("space"))
        {
            GameObject instance = Instantiate(coin, transform.position, transform.rotation);
            Destroy(instance, 2.0f);
        }
	}
}
