using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Playroom;
using UnityEngine.SceneManagement;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private MultiplayerManager _multiplayerManager;
    
    [SerializeField] private PlayerLobbyItem playerLobbyPrefab;
    [SerializeField] private Transform _playerLobbyParent;

    private Dictionary<string, PlayerLobbyItem> playerLobbyItems = new();

    private Dictionary<string,PlayroomKit.Player> PlayersList => PlayroomKit.GetPlayers();
    // Start is called before the first frame update
    void Awake()
    {
        _multiplayerManager.OnPlayerJoined += AddPlayerToLobby;
    }

    public void OnPlayroomInitialized()
    {
        PlayroomKit.OnPlayerJoin(AddPlayerToLobby);
    }

    private void AddPlayerToLobby(PlayroomKit.Player playerJoined)
    {
        var newPlayerUI = Instantiate(playerLobbyPrefab, _playerLobbyParent);
        newPlayerUI.Setup(playerJoined, PlayersList.Count-1);
        playerLobbyItems.Add(playerJoined.id, newPlayerUI);
    }

    public void ButtonStartGame()
    {
        SceneManager.LoadScene("GameScene");
    }
}
