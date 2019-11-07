using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuidebookManager : MonoBehaviour
{
    public GameObject unitsContent;
    public Text atk, introduce;
    public AudioClip selectSe;

    void Start()
    {
        Object[] units = Resources.LoadAll("Prefabs");

        foreach(GameObject unit in units)
        {
            Instantiate(unit, unitsContent.transform).transform.localScale = new Vector3(250, 250);
        }
    }

    public void ChangeInfo(UnitGuides target)
    {
        introduce.text = target.introduce;
    }
}
