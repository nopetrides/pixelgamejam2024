using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Entities/Entity")]
public class EntitySO : ScriptableObject
{
    [SerializeField]
    private int _health;

    [SerializeField]
    private int _attack;
    
    [SerializeField]
    private float _moveSpeed;

    public Sprite Icon;

    public int Health
    {
        get => _health;
        private set => _health = value;
    }

    public int Attack
    {
        get => _attack;
        private set => _attack = value;
    }

    public float MoveSpeed
    {
        get => _moveSpeed;
        private set => _moveSpeed = value;
    }
}
