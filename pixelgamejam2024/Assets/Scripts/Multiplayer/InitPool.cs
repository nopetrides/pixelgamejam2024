using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitPool : MonoBehaviour
{
    [SerializeField]
    private PoolableObject _poolableObject;

    private string _poolableObjectName;

    private void Start()
    {
        _poolableObjectName = _poolableObject.GetName();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PoolSystem.Instance.CreatePool(_poolableObjectName, _poolableObject);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PoolSystem.Instance.Spawn(_poolableObjectName, _poolableObject);
        }
    }
}
