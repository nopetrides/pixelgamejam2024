using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Progress : MonoBehaviour
{
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

    private void SetHeat(int obj)
    {
        
    }

    private void SetHunger(int obj)
    {
        
    }
}
