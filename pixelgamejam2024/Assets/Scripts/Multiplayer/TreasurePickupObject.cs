using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TreasurePickupObject : PoolableObject
{
    
    //setup the pulled treasure with the data from the SO
    [SerializeField] 
    private TreasureTypesSO _treasureData;

    [SerializeField]
    private SpriteRenderer _spriteRenderer;
    
    private int _weight;

    public override void DataSetup(Vector3 coordinates)
    {
        _treasureData = TreasureManager.Instance.GetTreasureDataFromCoordinates(coordinates);
        _weight = _treasureData.Weight;
        _name = _treasureData.Type;
        _spriteRenderer.sprite = _treasureData.Sprite;
    }

    private void OnTriggerEnter(Collider other)
    {
        TreasureManager.Instance.AddTreasureToPlayer(_weight);
        PoolSystem.Instance.DeSpawn("Treasure", this);
    }
}
