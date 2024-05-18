using System;
using Multiplayer;
using UnityEngine;
using Playroom;

public class MultiplayerManager : MonoBehaviour
{
    public Action OnPlayroomInit;
    public Action<PlayroomKit.Player> OnPlayerJoined;
    
    public void SetLobbyCodeAndStart(string lobbyCode)
    {
        StartLobby(lobbyCode);
    }

    public void HostNewLobby()
    {
        StartLobby();
    }

    private void StartLobby(string lobbyCode = null)
    {
        DontDestroyOnLoad(gameObject);
        var initOptions = new PlayroomKit.InitOptions()
        {
            roomCode = lobbyCode,
            gameId = "JCIWV1N4joeKV1xc5Cc4",
            maxPlayersPerRoom = 4,
            // Lobby-wide states
            defaultStates = new()
            {
                {GameConstants.GameStateData.Score.ToString(), 0}
            }, 
            // Player dependant states
            // Caution, these will reset any data set before a player joined our lobby.
            defaultPlayerStates = new()
            {
                {GameConstants.PlayerStateData.Position.ToString(), Vector3.zero},
                {GameConstants.PlayerStateData.IsCarrying.ToString(), false}
            },
            skipLobby = true, // we will make a custom lobby ui
            matchmaking = false, // true for creating auto-match lobbies with MatchmakingOptions
        };
        
        PlayroomKit.InsertCoin(initOptions, OnPlayerStartLobby, OnPlayerDisconnect);
    }

    void OnPlayerStartLobby()
    {
        Debug.Log($"Lobby created!");
        OnPlayroomInit?.Invoke();
        PlayroomKit.OnPlayerJoin(OnPlayerJoinLobby);
    }

    void OnPlayerDisconnect()
    {
        Debug.Log($"Disconnected from lobby.");
    }

    void OnPlayerJoinLobby(PlayroomKit.Player player)
    {
        Debug.Log($"{player.id} joined lobby!");
        OnPlayerJoined?.Invoke(player);
    }
}
