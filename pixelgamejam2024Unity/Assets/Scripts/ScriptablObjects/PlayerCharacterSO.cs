using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Entities/Player")]
public class PlayerCharacterSO : EntitySO
{
    [SerializeField]
    private float _carryCapacity;

    public float CarryCapacity
    {
        get => _carryCapacity;
        private set => _carryCapacity = value;
    }
}
