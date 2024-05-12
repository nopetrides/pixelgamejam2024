using Playroom;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The Lobby UI That let's them select a character
/// </summary>
public class PlayerLobbyItem : MonoBehaviour
{
    [SerializeField] private Image CharacterIcon;
    [SerializeField] private Color[] TempCharacterColors;

    private int _currentCharacterSelection;

    private PlayroomKit.Player _respresentsPlayer;
    private int _playerIndex;

    public void Setup(PlayroomKit.Player setupAsPlayer, int playerIndex)
    {
        _respresentsPlayer = setupAsPlayer;
        _playerIndex = playerIndex;

        CharacterIcon.color = TempCharacterColors[_playerIndex];
    }

}
