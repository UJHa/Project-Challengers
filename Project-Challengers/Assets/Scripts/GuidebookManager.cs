using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuidebookManager : MonoBehaviour
{
    public Text atk, introduce;
    public AudioClip selectSe;

    public void ChangeInfo(UnitGuides target)
    {
        atk.text = "공격 : " + target.atk;
        introduce.text = target.introduce;
    }
}
