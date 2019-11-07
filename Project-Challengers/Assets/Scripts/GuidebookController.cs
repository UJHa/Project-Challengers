using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuidebookController : MonoBehaviour
{
    static private GuidebookController selectMenu;

    public AudioClip buttonSe;
    public float moveSpeed = 1.0f;
    public GameObject content;

    private bool moving = false;

    private Color selectedColor = new Color(1.0f, 1.0f, 1.0f);
    private Color unselectedColor = new Color(0.6f, 0.6f, 0.6f);
    private Vector3 target;
    private AudioSource se;

    private void Start()
    {
        se = GameObject.Find("SE").GetComponent<AudioSource>();
        selectMenu = GameObject.Find("SummaryButton").GetComponent<GuidebookController>();
    }

    public void Update()
    {
        if (moving)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, target) == 0.0f)
            {
                moving = false;
            }
        }
    }

    public void Select()
    {
        GetComponent<Image>().color = selectedColor;
        BtnChange(this);
        content.SetActive(true);
        target = new Vector3(-220, transform.position.y);
        moving = true;
    }

    void Unselect()
    {
        GetComponent<Image>().color = unselectedColor;
        content.SetActive(false);
        target = new Vector3(-480, transform.position.y);
        moving = true;
    }

    void BtnChange(GuidebookController selected)
    {
        se.clip = buttonSe;
        se.Play();

        if (selectMenu != selected)
        {
            selectMenu.Unselect();
            selectMenu = selected;
        }
    }
}
