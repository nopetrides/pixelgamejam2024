using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TreasurePickupObject : PoolableObject
{
    
    //setup the pulled treasure with the data from the SO
    [SerializeField]
    private TreasureTypesSO _treasureData;

    private SpriteRenderer _spriteRenderer;
    
    private int _weight;
    private Sprite _sprite;
    //private string _type;

    

    protected override void OnEnable()
    {
        base.OnEnable();
        _weight = _treasureData.Weight;
        _sprite = _treasureData.Sprite;
        _name = _treasureData.Type;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = _sprite;
    }

    
}
