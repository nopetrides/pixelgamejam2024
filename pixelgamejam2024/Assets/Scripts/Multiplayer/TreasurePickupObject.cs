using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TreasurePickupObject : PoolableObject
{
    [SerializeField]
    private List<int> _weightAndValue;

    

    protected override void OnEnable()
    {
        base.OnEnable();
        PoolSystem.Instance.AddTreasure(_name, _weightAndValue);
    }

    
}
