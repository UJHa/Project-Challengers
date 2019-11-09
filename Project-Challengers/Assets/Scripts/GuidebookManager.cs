using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuidebookManager : MonoBehaviour
{
    public GameObject unitsContent, unitsPanel;
    public Text atk, introduce;
    public AudioClip selectSe;

    private string[] eCharacter =
    {
        "Blobminion",
        "Cyclops",
        "Detective",
        "Dwarf",
        "Imp",
        "Knight",
        "Lizard",
        "PlasmaDrone",
        "RoyalKnight",
        "Santa",
        "Skeleton",
        "SpaceCadet",
        "Taurus",
        "Vex"
    };

    void Start()
    {
        Object[] units = Resources.LoadAll("Prefabs");

        foreach(string character in eCharacter)
        {
            Debug.Log(character + "를 생성합니다");
            GameObject parent = Instantiate(unitsPanel, unitsContent.transform);
            Instantiate(Resources.Load<GameObject>("Prefabs/" + character), parent.transform).transform.localScale = new Vector3(200, 200);
            parent.GetComponentInChildren<Text>().text = character;
        }
    }

    public void ChangeInfo(UnitGuides target)
    {
        introduce.text = target.introduce;
    }
}
