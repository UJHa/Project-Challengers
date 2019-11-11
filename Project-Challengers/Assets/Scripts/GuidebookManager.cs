using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuidebookManager : MonoBehaviour
{
    public GameObject unitsContent, unitsPanel;
    public Text introduce, maxHp, moveSpeed, atkPower, findRange, atkRange;
    public AudioClip selectSe;

    private string[] eCharacter =
    {
        "BlobMinion",
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
            Instantiate(Resources.Load<GameObject>("Prefabs/Character/" + character), parent.transform).transform.localScale = new Vector3(200, 200);
            parent.GetComponentInChildren<Text>().text = character;
        }
    }

    public void ChangeInfo(GameObject target)
    {
        UnitGuides information = target.GetComponent<UnitGuides>();
        ChessCharacter stats = target.GetComponentInChildren<Skeleton>(true) ? (ChessCharacter)target.GetComponentInChildren<Skeleton>(true) : target.GetComponentInChildren<Knight>(true);

        introduce.text = information.introduce;
        maxHp.text = "최대체력 : " + stats.maxHp;
        moveSpeed.text = "이동속도 : " + stats.moveSpeed;
        atkPower.text = "공격력 : " + stats._attackPower;
        findRange.text = "탐색범위 : " + stats._findRange;
        atkRange.text = "공격범위 : " + stats._attackRange;
    }
}
