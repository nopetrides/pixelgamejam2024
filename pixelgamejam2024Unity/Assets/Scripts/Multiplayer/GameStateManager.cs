using System.Collections.Generic;
using Playroom;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    [SerializeField] private PlayerNetworkController PlayerPrefab;
    
    private Dictionary<string, PlayerNetworkController> Players = new();
    
    void Awake()
    {
        var players = PlayroomKit.GetPlayers();
        Debug.Log($"Loaded into the game with {players.Count} connected players");

        foreach (var p in players.Values)
        {
            var newPlayerObject = Instantiate(PlayerPrefab);
            newPlayerObject.Setup(p, this);
            Players.Add(p.id, newPlayerObject);
        }
    }
}
