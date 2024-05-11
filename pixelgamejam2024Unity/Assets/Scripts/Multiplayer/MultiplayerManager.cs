using System;
using UnityEngine;
using Playroom;

public class MultiplayerManager : MonoBehaviour
{
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
        var initOptions = new PlayroomKit.InitOptions()
        {
            roomCode = lobbyCode,
            gameId = "JCIWV1N4joeKV1xc5Cc4",
            maxPlayersPerRoom = 4,
            // Lobby-wide states
            defaultStates = new()
            {
                {"score", 0}
            }, 
            // Player dependant states
            defaultPlayerStates = new()
            {
                {"pos", Vector3.zero}
            },
            skipLobby = true, // we will make a custom lobby ui
            matchmaking = false, // true for creating auto-match lobbies with MatchmakingOptions
        };
        
        PlayroomKit.InsertCoin(initOptions, OnPlayerStartLobby, OnPlayerDisconnect);
    }

    void OnPlayerStartLobby()
    {
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
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
