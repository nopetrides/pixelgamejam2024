using System.Linq;
using Multiplayer;
using Playroom;
using UnityEngine;

public class PlayerNetworkController : MonoBehaviour
{
    [SerializeField] private PlayerController _controller;
    
    private PlayroomKit.Player _playroomPlayer;
    private GameStateManager _manager;
    
    
    public void Setup(PlayroomKit.Player player, GameStateManager manager)
    {
        _playroomPlayer = player;
        _manager = manager;
        var characterTypeState = player.GetState<GameConstants.CharacterTypes>(GameConstants.PlayerStateData.CharacterType.ToString());
        var characterData = PlayerCharactersData.Characters[characterTypeState];
        if (characterData == null)
        {
            Debug.LogError($"Failed to determine character for type {characterTypeState}");
            return;
        }
        SetAsCharacter(characterData);
    }

    private void SetAsCharacter(PlayerCharacterSO characterData)
    {
        _controller.SetAsCharacter(characterData);
    }
}
