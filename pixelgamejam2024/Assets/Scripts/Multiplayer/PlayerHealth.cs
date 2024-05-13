using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private Collider _collider;

    public int MaxHealth;
    private int _currentHealth;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        int healthChanger = HealthEffectsManager.Instance.CanCurrentlyAffectHealth(other.attachedRigidbody);
        if (healthChanger == 0) return;
        if (healthChanger > 0 && _currentHealth == MaxHealth) return;
        
        if (healthChanger > 0)
        {
            //Positive particles and anim
        }
        else
        {
            //Negative particles an anim
        }
        
        SetHealth(healthChanger);
    }

    private void SetHealth(int value)
    {
        _currentHealth += value;
    }
}
