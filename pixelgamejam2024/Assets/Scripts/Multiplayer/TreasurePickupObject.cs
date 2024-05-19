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

    public override void DataSetup(Vector3 coordinates)
    {
        //Debug.Log("TreasurePickupObject setup");
        _treasureData = TreasureManager.Instance.GetTreasureDataFromCoordinates(coordinates);
        _weight = _treasureData.Weight;
        _name = _treasureData.Type;
        _spriteRenderer.sprite = _treasureData.Sprite;
    }

    private void OnTriggerEnter(Collider other)
    {
        // This was caused by the players trigger (at least when it is first picked up)
        // todo inform the treasure manager, who then informs the player.
        // let the treasure manager handle returning the treasure to the pool - will also tell network players this treasure was picked up.
        // let the player handle adding weight to their inventory (if they can pick it up)
        // TODO handle dropping in the pool
    }
}
