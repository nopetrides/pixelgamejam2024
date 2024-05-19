using System.Collections.Generic;
using Multiplayer;
using Playroom;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    [SerializeField] private Transform DragonTurtleLocation;
    [SerializeField] private Transform[] SpawnLocator;
    [SerializeField] private PlayerNetworkControllerV2 PlayerPrefab;
    [SerializeField] private MapChunksManager MapManager;
    [SerializeField] private GameUI _gameUI;
    
    private Dictionary<string, PlayerNetworkControllerV2> Players = new();
    
    void Start()
    {
        var players = PlayroomKit.GetPlayersOrNull();

        if (players == null)
        {
            // we didn't set up playroom first, so lets fake it for testing purposes
            var startPos = SpawnLocator[0].position;
            Vector3 direction = (DragonTurtleLocation.position - startPos).normalized;
            var newPlayerObject = Instantiate(PlayerPrefab, startPos, Quaternion.LookRotation(direction));
            _gameUI.SetPlayer(newPlayerObject);
            return;
        }
        
        Debug.Log($"Loaded into the game with {players.Count} connected players");

        foreach (var p in players.Values)
        {
            var characterTypeState = p.GetState<int>(GameConstants.PlayerStateData.CharacterType.ToString());
            var startPos = SpawnLocator[characterTypeState].position;
            if (PlayroomKit.Me() == p) 
                p.SetState(GameConstants.PlayerStateData.Position.ToString(), transform.position);
            Vector3 direction = (DragonTurtleLocation.position - startPos).normalized;
            var newPlayerObject = Instantiate(PlayerPrefab, startPos, Quaternion.LookRotation(direction));
            newPlayerObject.Setup(p, this);
            Players.Add(p.id, newPlayerObject);
        }

        TreasureManager.Instance.Initialize();
        
        MapManager.enabled = true;
        MapManager.InitializeMapChunks();
        
        
    }
}
