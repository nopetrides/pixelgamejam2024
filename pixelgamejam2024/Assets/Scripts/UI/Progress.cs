using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Progress : MonoBehaviour
{
    [SerializeField]
    private int _hunger;
    [SerializeField]
    private int _heat;

    
    private void OnEnable()
    {
        Dragon.Hunger += SetHunger;
        Dragon.Heat += SetHeat;
    }

    private void OnDisable()
    {
        Dragon.Hunger -= SetHunger;
        Dragon.Heat -= SetHeat;
    }
    
    private void SetHunger(int obj)
    {
        _hunger += obj;
        SetHeat((int)(obj * 0.75f));
    }

    private void SetHeat(int obj)
    {
        _heat += obj;
    }

    //For Testing
    private void Update()
    {
        //if(Input.GetKeyDown(KeyCode.Alpha1)) SetHunger(25);
    }
}
