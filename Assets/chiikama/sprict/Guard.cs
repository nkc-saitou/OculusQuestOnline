using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Guard : MonoBehaviour
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
        if (col.GetComponent<Matsumoto.Weapon.ModuleObject>() != null)
        {
            currentHP--;
        }

        if(currentHP <= 0)
        {
            Destroy(gameObject);
            DestroyAct();
        }
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
