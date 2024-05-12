using System.Collections.Generic;
using Multiplayer;
using Playroom;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    [SerializeField] private PlayerNetworkControllerV2 PlayerPrefab;
    
    private Dictionary<string, PlayerNetworkControllerV2> Players = new();
    
    void Awake()
    {
        var players = PlayroomKit.GetPlayers();
        Debug.Log($"Loaded into the game with {players.Count} connected players");

        foreach (var p in players.Values)
        {
            Debug.Log($"[GameStateManager] 1");
            var startPos = Vector3.zero;
            Debug.Log($"[GameStateManager] 2");
            var newPlayerObject = Instantiate(PlayerPrefab, startPos, Quaternion.identity);
            Debug.Log($"[GameStateManager] 3");
            newPlayerObject.Setup(p, this);
            Debug.Log($"[GameStateManager] 4");
            Players.Add(p.id, newPlayerObject);
            Debug.Log($"[GameStateManager] 5");
        }
    }
}
