using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    public Text resultText;

    // Start is called before the first frame update
    void Start()
    {
        if (true)
        {
            resultText.text = "승리!";
        }
        else
        {
            resultText.text = "패배";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
