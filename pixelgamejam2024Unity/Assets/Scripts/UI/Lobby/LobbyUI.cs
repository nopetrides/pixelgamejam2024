using System;
using System.Collections.Generic;
using AOT;
using UnityEngine;
using Playroom;
using TMPro;
using UnityEngine.SceneManagement;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private MultiplayerManager _multiplayerManager;
    [SerializeField] private MainMenuUI _mainMenu;
    
    [Header("Players")]
    [SerializeField] private PlayerLobbyItem playerLobbyPrefab;
    [SerializeField] private Transform _playerLobbyParent;

    [Header("Other UI")] 
    [SerializeField] private TMP_Text LobbyCode;
    
    private Dictionary<string, PlayerLobbyItem> _playerLobbyItems = new();

    private Dictionary<string,PlayroomKit.Player> PlayersList => PlayroomKit.GetPlayers();

    void Awake()
    {
        _multiplayerManager.OnPlayerJoined += AddPlayerToLobby;
        gameObject.SetActive(false);
    }

    
    private void AddPlayerToLobby(PlayroomKit.Player playerJoined)
    {
        var newPlayerUI = Instantiate(playerLobbyPrefab, _playerLobbyParent);
        newPlayerUI.Setup(playerJoined, PlayersList.Count-1);
        _playerLobbyItems.Add(playerJoined.id, newPlayerUI);
        playerJoined.OnQuit(RemovePlayer);

        LobbyCode.text = PlayroomKit.IsRunningInBrowser() ? PlayroomKit.GetRoomCode() : "Mock Mode";
    }
    

    [MonoPInvokeCallback(typeof(Action<string>))]
    private void RemovePlayer(string playerID)
    {
        var itemToRemove = _playerLobbyItems[playerID];
        Destroy(itemToRemove.gameObject);
        _playerLobbyItems.Remove(playerID);
    }

    public void ButtonStartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void ButtonBack()
    {
        PlayroomKit.UnsubscribeOnQuit();
        gameObject.SetActive(false);
        foreach (var item in _playerLobbyItems.Values)
        {
            Destroy(item.gameObject);
        }
        _playerLobbyItems.Clear();
        
        _mainMenu.ShowMenu();
    }
}
