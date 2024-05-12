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
    [SerializeField] private Image CharacterBG;
    [SerializeField] private Image CharacterIcon;
    [SerializeField] private GameObject ControlsParent;
    [SerializeField] private TMP_Text CharacterNameText;

    private int _currentCharacterSelection;

    private PlayroomKit.Player _representsPlayer;
    private LobbyUI _manager;

    public void Setup(PlayroomKit.Player setupAsPlayer, LobbyUI manager)
    {
        _representsPlayer = setupAsPlayer;
        _manager = manager;
        ControlsParent.SetActive(PlayroomKit.Me() == setupAsPlayer);
        ChangeCharacter();
    }

    public void ButtonPreviousCharacter()
    {
        var character = CurrentCharacter();
        var characterType = character.CharacterType.Previous();
        _representsPlayer.SetState(GameConstants.PlayerStateData.CharacterType.ToString(), (int)characterType, true);
        ChangeCharacter();
        _manager.RefreshLobby();
    }

    public void ButtonNextCharacter()
    {
        var character = CurrentCharacter();
        var characterType = character.CharacterType.Next();
        _representsPlayer.SetState(GameConstants.PlayerStateData.CharacterType.ToString(), (int)characterType, true);
        ChangeCharacter();
        _manager.RefreshLobby();
    }

    public void RefreshUI()
    {
        ChangeCharacter();
    }
    
    private void ChangeCharacter()
    {
        var character = CurrentCharacter();
        
        // change visual to match character
        CharacterBG.color = character.CharacterColor;
        CharacterIcon.color = character.CharacterColor;
        CharacterNameText.text = character.CharacterType.ToString();
    }

    private PlayerCharacterSO CurrentCharacter()
    {
        var characterTypeState = _representsPlayer.GetState<int>(GameConstants.PlayerStateData.CharacterType.ToString());
        var characterData = PlayerCharactersData.Characters[(GameConstants.CharacterTypes)characterTypeState];
        if (characterData == null)
        {
            Debug.LogError($"Failed to determine character for type {characterTypeState}");
            return PlayerCharactersData.Characters[GameConstants.CharacterTypes.Alpha];
        }

        return characterData;
    }
}
