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
            var startPos = p.GetState<Vector3>(GameConstants.PlayerStateData.Position.ToString());
            var newPlayerObject = Instantiate(PlayerPrefab, startPos, Quaternion.identity);
            newPlayerObject.Setup(p, this);
            Players.Add(p.id, newPlayerObject);
        }
    }
}
