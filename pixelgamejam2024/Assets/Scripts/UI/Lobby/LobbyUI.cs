using System;
using System.Collections.Generic;
using AOT;
using Multiplayer;
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

    void Awake()
    {
        _multiplayerManager.OnPlayerJoined += AddPlayerToLobby;
        _multiplayerManager.OnPlayroomInit += PlayroomInit;
        gameObject.SetActive(false);
        StartGameButton.gameObject.SetActive(false);
    }


    private void OnDestroy()
    {
        if (_multiplayerManager != null)
        {
            _multiplayerManager.OnPlayerJoined -= AddPlayerToLobby;
            _multiplayerManager.OnPlayroomInit -= PlayroomInit;
        }
    }

    private void PlayroomInit()
    {
        if (PlayroomKit.IsRunningInBrowser())
        {
            Debug.Log("Registering Start Game Callback");
            PlayroomKit.RpcRegister("Start", StartGameRPC, "Loading Response");
            PlayroomKit.RpcRegister("RefreshLobby", OnRefreshLobbyRPC, "Refreshing Lobby...");
            //PlayroomKit.RpcRegister("JoinedLobby", OnLobbyDataRPC, "Joined the lobby and requesting updates...");
        }
        StartGameButton.gameObject.SetActive(PlayroomKit.IsHost());
        LobbyCode.text = PlayroomKit.IsRunningInBrowser() ? PlayroomKit.GetRoomCode() : "Mock Mode";
    }

    private void AddPlayerToLobby(PlayroomKit.Player playerJoined)
    {
        if (_playerLobbyItems.ContainsKey(playerJoined.id))
        {
            Debug.Log($"Player already in lobby {playerJoined.id}");
            return;
        }
        Debug.Log($"Player joined lobby {playerJoined.id}");
        if (PlayroomKit.IsRunningInBrowser())
        {
            var self = PlayroomKit.Me();
            if (self == playerJoined)
            {
                // We added a player to our local lobby, we should update with all the states from the other players
                playerJoined.SetState(GameConstants.PlayerStateData.CharacterType.ToString(), (int)GameConstants.CharacterTypes.Alpha, true);
                CreatePlayerLobbyItem(playerJoined);
                //RequestLobbyPlayerStates();
            }
            else
            {
                CreatePlayerLobbyItem(playerJoined);
            }
        }
        else
        {
            playerJoined.SetState(GameConstants.PlayerStateData.CharacterType.ToString(), (int)GameConstants.CharacterTypes.Alpha, true);
            CreatePlayerLobbyItem(playerJoined);
        }
    }

    private void CreatePlayerLobbyItem(PlayroomKit.Player player)
    {
        var newPlayerUI = Instantiate(playerLobbyPrefab, _playerLobbyParent);

        newPlayerUI.Setup(player, this);
        _playerLobbyItems.Add(player.id, newPlayerUI);
        player.OnQuit(RemovePlayer);
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
            StartGameRPC("GameScene", PlayroomKit.Me().id);
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
    
    private void StartGameRPC(string data, string senderId)
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
        Debug.Log($"[RefreshLobby] Sending Refresh message");
        if (PlayroomKit.IsRunningInBrowser()) PlayroomKit.RpcCall("RefreshLobby", "Change Character", PlayroomKit.RpcMode.OTHERS, LobbyRefreshConfirmedCallback);
    }

    private void OnRefreshLobbyRPC(string data, string senderId)
    {
        Debug.Log($"[OnRefreshLobbyRPC]" + data);
        _playerLobbyItems[senderId].RefreshUI();
    }

    private void LobbyRefreshConfirmedCallback()
    {
        Debug.Log("Refresh Confirmed");
    }

    /*
    private void RequestLobbyPlayerStates()
    {
        PlayroomKit.RpcCall("JoinedLobby", "New Player Joined", PlayroomKit.RpcMode.OTHERS, LobbyRefreshConfirmedCallback);
    }

    private void OnLobbyDataRPC(string data, string senderId)
    {
        var self = PlayroomKit.Me();
        string key = GameConstants.PlayerStateData.CharacterType.ToString();
        self.SetState(key, self.GetState<int>(key), true);
        PlayroomKit.RpcCall("RefreshLobby", "Change Character", PlayroomKit.RpcMode.OTHERS, LobbyDataRequestConfirmed);
    }
    
    private void LobbyDataRequestConfirmed()
    {
        Debug.Log("Data Request Confirmed");
    }
    */
}
