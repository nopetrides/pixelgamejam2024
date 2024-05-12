using System;
using System.Linq;
using Multiplayer;
using Playroom;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The Lobby UI That let's them select a character
/// </summary>
public class PlayerLobbyItem : MonoBehaviour
{
    [SerializeField] private Image CharacterIcon;
    [SerializeField] private Color[] TempCharacterColors;
    [SerializeField] private TMP_Text CharacterNameText;

    private int _currentCharacterSelection;

    private PlayroomKit.Player _representsPlayer;
    private int _playerIndex;

    public void Setup(PlayroomKit.Player setupAsPlayer, int playerIndex)
    {
        _representsPlayer = setupAsPlayer;
        _playerIndex = playerIndex;
        _representsPlayer.SetState(GameConstants.PlayerStateData.CharacterType.ToString(), (GameConstants.CharacterTypes)_playerIndex);
        
        ChangeCharacter();
    }

    public void ButtonPreviousCharacter()
    {
        var characterType = CurrentCharacter();
        characterType = characterType.Previous();
        _representsPlayer.SetState(GameConstants.PlayerStateData.CharacterType.ToString(), characterType);
        ChangeCharacter();
    }

    public void ButtonNextCharacter()
    {
        var characterType = CurrentCharacter();
        characterType = characterType.Next();
        _representsPlayer.SetState(GameConstants.PlayerStateData.CharacterType.ToString(), characterType);
        ChangeCharacter();
    }
    
    private void ChangeCharacter()
    {
        var characterType = CurrentCharacter();
        
        // change visual to match character
        CharacterIcon.color = TempCharacterColors[(int)characterType];
        CharacterNameText.text = characterType.ToString();
    }

    private GameConstants.CharacterTypes CurrentCharacter()
    {
        var characterTypeState = _representsPlayer.GetState<GameConstants.CharacterTypes>(GameConstants.PlayerStateData.CharacterType.ToString());
        var characterData = PlayerCharactersData.Characters[characterTypeState];
        if (characterData == null)
        {
            Debug.LogError($"Failed to determine character for type {characterTypeState}");
            return GameConstants.CharacterTypes.Alpha;
        }

        return characterData.CharacterType;
    }
}
