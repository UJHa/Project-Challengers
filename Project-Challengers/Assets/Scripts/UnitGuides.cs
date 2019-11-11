using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitGuides : MonoBehaviour
{
    public string introduce;

    private GameObject unitGuide;
    private GuidebookManager guidebookManager;
    private Animator animator, selectAnimator;
    private Text guideName;
    private AudioSource se;


    // Start is called before the first frame update
    void Start()
    {
        se = GameObject.Find("SE").GetComponent<AudioSource>();

        unitGuide = GameObject.Find("UnitGuide");
        guidebookManager = GameObject.Find("GuidebookManager").GetComponent<GuidebookManager>();

        selectAnimator = GetComponentInChildren<Animator>();

        animator = unitGuide.GetComponentInChildren<Animator>();
        guideName = unitGuide.GetComponentInChildren<Text>();
    }

    public void SelectGuide()
    {
        se.clip = guidebookManager.selectSe;
        se.Play();

        animator.runtimeAnimatorController = selectAnimator.runtimeAnimatorController;
        guideName.text = gameObject.GetComponentInChildren<Text>().text;
        guidebookManager.ChangeInfo(gameObject);
    }
}
