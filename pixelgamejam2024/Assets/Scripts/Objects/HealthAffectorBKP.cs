using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HealthAffectorBKP : MonoBehaviour
{
    [SerializeField]
    private HealthAffectorSO _healthAffectorSo;
    
    private Rigidbody _rb;
    private int _healthChangeValue;

    private void Awake()//Working; will need logic on instantiating to check if the pool exists, and, if not, create new.
    {
        _rb = GetComponent<Rigidbody>();
        _healthChangeValue = _healthAffectorSo.HealthChangeValue;
        HealthEffectsManager.Instance.CreatePool(_rb, _healthChangeValue, 1);
    }

    public Rigidbody GetRigidbody()
    {
        return _rb;
    }

    public int GetHealthChangeValue()
    {
        return _healthChangeValue;
    }
}
