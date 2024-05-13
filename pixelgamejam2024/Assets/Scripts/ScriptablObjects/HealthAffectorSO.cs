using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Objects/HealthAffector")]
public class HealthAffectorSO : ScriptableObject
{
    [SerializeField]
    private int _healthChangeValue;

    public int HealthChangeValue
    {
        get => _healthChangeValue;
        private set => _healthChangeValue = value;
    }
}
