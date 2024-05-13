using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HealthAffector : MonoBehaviour
{
    [SerializeField]
    private HealthAffectorSO _healthAffectorSo;
    
    private Rigidbody _rb;
    private int _healthChangeValue;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _healthChangeValue = _healthAffectorSo.HealthChangeValue;
        gameObject.SetActive(false);
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
