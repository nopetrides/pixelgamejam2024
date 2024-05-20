using System.Collections.Generic;
using Multiplayer;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Entities/PlayerData")]
public class PlayerCharactersData : ScriptableObject
{
    [SerializeField] private PlayerCharacterSO[] _characterTypes;

    private static Dictionary<GameConstants.CharacterTypes, PlayerCharacterSO> _charactersDictionary;
    public static Dictionary<GameConstants.CharacterTypes, PlayerCharacterSO> Characters => _charactersDictionary;

    public PlayerCharacterSO[] CharacterTypes
    {
        get => _characterTypes;
        private set => _characterTypes = value;
    }
    
    private void Awake()
    {
        if (!Application.isPlaying) return;
        _charactersDictionary = new();
        foreach(var c in _characterTypes) 
            _charactersDictionary.Add(c.CharacterType, c);
        
    }
}