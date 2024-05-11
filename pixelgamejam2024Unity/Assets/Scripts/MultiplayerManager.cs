using System;
using UnityEngine;
using Playroom;

public class MultiplayerManager : MonoBehaviour
{
    public Action<PlayroomKit.Player> OnPlayerJoined;
    
    // Start is called before the first frame update
    void Start()
    {
        var initOptions = new PlayroomKit.InitOptions()
        {
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
