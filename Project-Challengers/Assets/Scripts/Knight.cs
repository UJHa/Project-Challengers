using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Knight : ChessCharacter
{
    public GameObject weapon;
    protected GameObject weaponObject;

    protected override void InitData()
    {
        base.InitData();

        // 무기 초기 세팅
        weaponObject = Instantiate(weapon) as GameObject;
        weaponObject.transform.SetParent(transform, false);
    }

    protected override void InitState()
    {
        base.InitState();
    }

    public override void AttackStart()
    {
        weaponObject.transform.localEulerAngles = Vector3.zero;
    }

    public override void AttackUpdate()
    {
        weaponObject.transform.localEulerAngles = Vector3.Slerp(weaponObject.transform.localEulerAngles, new Vector3(0, 0, -90), Time.deltaTime * 1.0f);
        //Debug.Log("attacking log : " + weaponObject.transform.localEulerAngles.z);
        if (weaponObject.transform.localEulerAngles.z < 260)
        {
            weaponObject.transform.localEulerAngles = weapon.transform.eulerAngles;
            SetState(eState.IDLE);
        }
    }
}
