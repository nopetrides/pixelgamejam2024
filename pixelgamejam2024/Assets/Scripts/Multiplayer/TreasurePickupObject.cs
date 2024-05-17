using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TreasurePickupObject : PoolableObject
{
    
    //setup the pulled treasure with the data from the SO
    private TreasureTypesSO _treasureData;

    [SerializeField]
    private SpriteRenderer _spriteRenderer;
    
    private int _weight;
    //private string _type;

    public TreasureTypesSO TreasureData => _treasureData;

    public override void DataSetup(string data)
    {
        _treasureData = TreasureManager.Instance.Deserializer(data);
        Debug.Log($"Treasuer: {transform.position}");
        _weight = _treasureData.Weight;
        _name = _treasureData.Type;
        _spriteRenderer.sprite = _treasureData.Sprite;
    }
}
