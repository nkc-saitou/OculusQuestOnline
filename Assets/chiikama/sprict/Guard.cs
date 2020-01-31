using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

public class Guard : MonoBehaviourPunCallbacks
{
    public int HP
    {
        get;set;
    }

    public int MyIndex
    {
        get;set;
    }

    public Action DestroyAct;

    private int currentHP = 0;

    void OnTriggerEnter(Collider col)
    {
        if (TestOnlineData.PlayerID == 2) return;

        if (col.GetComponent<Matsumoto.Weapon.ModuleObject>() != null)
        {
            currentHP--;
        }

        if(currentHP <= 0)
        {
            GuardDestroy();
        }
    }

    public void GuardDestroy()
    {
        PhotonNetwork.Destroy(gameObject);
        DestroyAct();
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHP = HP;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
