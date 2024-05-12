using System.Collections;
using System.Collections.Generic;
using CMF;
using Multiplayer;
using Playroom;
using UnityEngine;

public class PlayerNetworkControllerV2 : MonoBehaviour
{
    [SerializeField] private AdvancedWalkerController _controller;
    // todo, player controller that handles the other game mechanics
    
    private PlayroomKit.Player _playroomPlayer;
    private GameStateManager _manager;
    
    
    public void Setup(PlayroomKit.Player player, GameStateManager manager)
    {
        _playroomPlayer = player;
        _manager = manager;
        var characterTypeState = player.GetState<int>(GameConstants.PlayerStateData.CharacterType.ToString());
        var characterData = PlayerCharactersData.Characters[(GameConstants.CharacterTypes)characterTypeState];
        if (characterData == null)
        {
            Debug.LogError($"Failed to determine character for type {characterTypeState}");
            return;
        }
        SetAsCharacter(characterData);
    }

    private void SetAsCharacter(PlayerCharacterSO characterData)
    {
        _controller.movementSpeed = characterData.MoveSpeed;
        // todo, other data driven fields - modify a different controller that handles player stats like hp and carrying.
    }
}
