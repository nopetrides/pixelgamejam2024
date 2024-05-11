using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Objects/Pickup")]
public class PickupSO : ScriptableObject
{
    [SerializeField]
    private float _weight;

    [SerializeField]
    private float _hungerSatiation;

    [SerializeField]
    private float _heatChange;

    public Sprite Sprite;

    public float Weight
    {
        get => _weight;
        private set => _weight = value;
    }

    public float HungerSatiation
    {
        get => _hungerSatiation;
        private set => _hungerSatiation = value;
    }

    public float HeatChange
    {
        get => _heatChange;
        private set => _heatChange = value;
    }
}
