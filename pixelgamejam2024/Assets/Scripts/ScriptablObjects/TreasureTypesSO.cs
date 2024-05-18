using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Objects/Treasure")]
public class TreasureTypesSO : ScriptableObject
{
    [SerializeField]
    private int _weight;

    [SerializeField]
    private Sprite _sprite;

    [SerializeField]
    private string _type;

    public int Weight
    {
        get => _weight;
        private set => _weight = value;
    }
    
    public Sprite Sprite
    {
        get => _sprite;
        private set => _sprite = value;
    }
    
    public string Type
    {
        get => _type;
        private set => _type = value;
    }
}
