using System;
using System.Collections.Generic;
using AOT;
using UnityEngine;
using Playroom;
using TheraBytes.BetterUi;
using TMPro;
using UnityEngine.SceneManagement;

public class LobbyUI : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private MultiplayerManager _multiplayerManager;
    [SerializeField] private MainMenuUI _mainMenu;
    
    [Header("Players")]
    [SerializeField] private PlayerLobbyItem playerLobbyPrefab;
    [SerializeField] private Transform _playerLobbyParent;

    [Header("Other UI")] 
    [SerializeField] private TMP_Text LobbyCode;
    [SerializeField] private BetterButton StartGameButton;
    
    private Dictionary<string, PlayerLobbyItem> _playerLobbyItems = new();

    private Dictionary<string,PlayroomKit.Player> PlayersList => PlayroomKit.GetPlayers();

    void Awake()
    {
        _multiplayerManager.OnPlayerJoined += AddPlayerToLobby;
        _multiplayerManager.OnPlayroomInit += PlayroomInit;
        gameObject.SetActive(false);
        StartGameButton.gameObject.SetActive(false);
    }

    private void PlayroomInit()
    {
        if (PlayroomKit.IsRunningInBrowser())
        {
            Debug.Log("Registering Start Game Callback");
            PlayroomKit.RpcRegister("Start", StartGameCallback, "Loading Response");
            PlayroomKit.RpcRegister("RefreshLobby", OnRefreshLobbyRPC, "Refreshing Lobby..");
        }
        StartGameButton.gameObject.SetActive(PlayroomKit.IsHost());
    }

    private void AddPlayerToLobby(PlayroomKit.Player playerJoined)
    {
        if (_playerLobbyItems.ContainsKey(playerJoined.id))
        {
            Debug.Log($"Player already in lobby {playerJoined.id}");
            return;
        }
        Debug.Log($"Player joined lobby {playerJoined.id}");
        var newPlayerUI = Instantiate(playerLobbyPrefab, _playerLobbyParent);
        newPlayerUI.Setup(playerJoined, this);
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
        Debug.Log("Start Game Pressed!");
        if (!PlayroomKit.IsRunningInBrowser())
        {
            StartGameCallback("GameScene", PlayroomKit.Me().id);
            return;
        }
        if (PlayroomKit.IsHost())
        {
            Debug.Log("Calling RPC to start game!");
            PlayroomKit.RpcCall("Start", "GameScene", PlayroomKit.RpcMode.ALL, StartGameConfirmCallback);
        }
        else 
            Debug.LogError("[ButtonStartGame] Hey, how did you get to this?");
    }
    
    private void StartGameCallback(string data, string senderId)
    {
        Debug.Log($"Received data: {data}");
        SceneManager.LoadScene(data.Trim('\"','\''));
    }

    private void StartGameConfirmCallback()
    {
        Debug.Log("Loading Confirmed");
    }

    public void ButtonBack()
    {
        PlayroomKit.Me().Kick();
        gameObject.SetActive(false);
        foreach (var item in _playerLobbyItems.Values)
        {
            Destroy(item.gameObject);
        }
        _playerLobbyItems.Clear();
        
        _mainMenu.ShowMenu();
    }

    public void RefreshLobby()
    {
        if (PlayroomKit.IsRunningInBrowser()) PlayroomKit.RpcCall("RefreshLobby", "Change Character", PlayroomKit.RpcMode.ALL, LobbyRefreshConfirmedCallback);
    }

    private void OnRefreshLobbyRPC(string data, string senderId)
    {
        _playerLobbyItems[senderId].RefreshUI();
    }

    private void LobbyRefreshConfirmedCallback()
    {
        Debug.Log("Refresh Confirmed");
    }
}
