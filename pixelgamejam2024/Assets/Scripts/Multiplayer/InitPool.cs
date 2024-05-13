using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitPool : MonoBehaviour
{
    [SerializeField]
    private HealthAffector[] _healthAffectors;

    private void Start()
    {
        // for (int i = 0; i < _healthAffectors.Length; i++)
        // {
        //     HealthEffectsManager.Instance.CreatePool(_healthAffectors[i].GetRigidbody(), _healthAffectors[i].GetHealthChangeValue(), 10);
        // }
    }

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Alpha1)) Instantiate(_healthAffectors[0]);
        // if (Input.GetKeyDown(KeyCode.Alpha2)) Instantiate(_healthAffectors[1]);
    }
}
