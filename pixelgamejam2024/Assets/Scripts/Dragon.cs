using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : MonoBehaviour
{
    public static event Action<int> Hunger;
    public static event Action<int> Heat;

    private void OnTriggerEnter(Collider other)
    {
        //Detect feeding
        NotifyHunger();
        
        //Detect cooling or heating
        NotifyHeat();
    }

    private void NotifyHunger()
    {
        //Hunger?.Invoke(hungervalue);
    }

    private void NotifyHeat()
    {
        //Heat?.Invoke(heatvalue);
    }
}
