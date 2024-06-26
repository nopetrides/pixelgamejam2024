using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Playroom;
using UnityEngine;

public class TreasureManager : MonoBehaviour
{
    // Using RpcCall RpcMode.HOST, send the coordinates of the treasure spawner.
    // The host keeps a dictionary of all activated treasure spawners.
    // If this is a new unique location (Vector3 position) then the host does a RpcCall RpcMode.ALL to spawn the item on all players
    // Each player that receives this message spawns the treasure item at the given coordinates

    // When a player picks up an item, tell all players to de-spawn it, that player will manager its state locally (either in inventory or "carried")
    
    // When a player drops an item, tell all players to spawn it back into their world at the dropped coordinates.
        // maybe a special condition if they drop it into the treasure pool?
    
    // Aside: Check if a treasure item is in a loaded chunk, and if not, don't render it?
    
    public static TreasureManager Instance;

    public PlayerTreasurePickup _localPlayer { get; private set; }

    [SerializeField]
    private List<TreasureTypesSO> _treasureTypes;

    [SerializeField]
    private List<string> _treasureNames;

    private ConcurrentDictionary<string, TreasureTypesSO> _treasureNamesTypes = new ();

    private ConcurrentBag<TreasureTypesSO> _immutableTreasure;

    /// <summary>
    /// Each treasure spawner has a unique id so we can track
    /// that unique id is based on their world coordinates which will be the same between players
    /// regardless of the order they uncover the map
    /// </summary>
    private ConcurrentDictionary<Vector3, string> TreasureSpawner = new();

    public void SetLocalPlayerPickup(PlayerTreasurePickup player) => _localPlayer = player;

    public bool AddTreasureToPlayer(int weight, Vector3 location)
    {
        if (_localPlayer.GetWeight() + weight > _localPlayer.GetCarryLimit()) return false;
        _localPlayer.AddToWeight(weight, location);
        return true;
    } 
        

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (!PlayroomKit.IsRunningInBrowser())
        {
            Debug.Log("[TreasureManager] Awake - Skipping playroom initialization.");
            Initialize();
        }
    }
    public void Initialize()
    {
        //Debug.Log(_treasureTypes.Count);
        for (int i = 0; i < _treasureNames.Count && i < _treasureTypes.Count; i++)
        {
            _treasureNamesTypes.TryAdd(_treasureNames[i], _treasureTypes[i]);
        }
        _immutableTreasure = new(_treasureTypes);
        
        // all player will call this rpc to ask the server if this treasure spawner was already found
        if (!PlayroomKit.IsRunningInBrowser()) return;
        PlayroomKit.RpcRegister("FoundTreasureSpawner", DoesTreasureSpawnerExistInDictionary, "Found Spawner message success");
        // Only the host will call this rpc to tell all players to spawn a new treasure object at a location.
        PlayroomKit.RpcRegister("SpawnTreasureEveryone", AddNewTreasureToDictionary, "Treasure created confirmed");
        // Tell others players we picked something up
        PlayroomKit.RpcRegister("TreasurePickup", RemovePickedUpTreasure, "Treasure pickup informed");
    }

    /// <summary>
    ///     Comes from the map node, all players will call this
    /// </summary>
    /// <param name="treasureSpawnerCoordinates"></param>
    public void AskServerIfTreasureSpawnerWasAlreadyFound(Vector3 treasureSpawnerCoordinates)
    {
        if (!PlayroomKit.IsRunningInBrowser())
        {
            DoesTreasureSpawnerExistInDictionary(JsonUtility.ToJson(treasureSpawnerCoordinates), "");
        }
        else
        {
            if (PlayroomKit.IsHost())
                DoesTreasureSpawnerExistInDictionary(JsonUtility.ToJson(treasureSpawnerCoordinates), "");
            else 
                PlayroomKit.RpcCall("FoundTreasureSpawner", treasureSpawnerCoordinates, PlayroomKit.RpcMode.HOST, () => Debug.Log("Treasure spawner check confirmed"));
        }
    }
    
    public void DoesTreasureSpawnerExistInDictionary(string coordinates, string _)
    {
        Debug.Log($"Checking dictionary " + coordinates);
        // check dictionary
        Vector3 coords = JsonUtility.FromJson<Vector3>(coordinates); //Vector3Parser.TryParse(coordinates, out Vector3 result) ? result : Vector3.zero;
        bool inDictionary = TreasureSpawner.ContainsKey(coords);
        if (inDictionary) return;

        // if does not yet exist:
        // tell all player to spawn this thing, and add it to their local dictionary
        var spawnerID = "Spawner_ID_" + coords;
        TreasureSpawner.TryAdd(coords, spawnerID);
        
        //logic to randomize treasure before spawning

        //var treasureData = _immutableTreasure.ToArray()[Random.Range(0, _immutableTreasure.Count)];
        //Debug.Log($"{treasureData.Type}");
        //Dictionary<string,string> messageData = new (){{"Coordinates",coordinates},{"Type",treasureData.Type}};
        //var messageData = new TreasureDataSerializer(){Coordinates = coords, Type = treasureData.Type};
        //string serializedTreasureData = JsonUtility.ToJson(messageData);
        
        
        if (!PlayroomKit.IsRunningInBrowser())
        {
            AddNewTreasureToDictionary(JsonUtility.ToJson(coords), "");
        }
        else if (PlayroomKit.IsHost())
        {
            AddNewTreasureToDictionary(JsonUtility.ToJson(coords), "");
            PlayroomKit.RpcCall("SpawnTreasureEveryone", coords, PlayroomKit.RpcMode.OTHERS, () => Debug.Log("Treasure spawn confirmed"));
        }
    }
    
    private void AddNewTreasureToDictionary(string coordinates, string _)
    {
        // Server has told us to make a new treasure in the world.
        // add to to the object pool
        // get next pooled object and show it
        // todo, coordinates where the new treasure item spawns
        Debug.Log($"Adding to dictionary");
        Vector3 coords = JsonUtility.FromJson<Vector3>(coordinates); // Vector3Parser.TryParse(coordinates, out Vector3 result) ? result : Vector3.zero;
        PoolSystem.Instance.Spawn("Treasure", coords);
    }

    public TreasureTypesSO GetTreasureDataFromCoordinates(Vector3 coordinates)
    {
        uint seed = Convert.ToUInt32(Mathf.RoundToInt(Mathf.Abs(coordinates.x)) * Mathf.RoundToInt(Mathf.Abs(coordinates.z)));
        var rando = new Unity.Mathematics.Random(seed != 0 ? seed+1010 : 1010 );
        var _ = rando.NextInt(0, _immutableTreasure.Count); // throwout the first one
        var ranInt = rando.NextInt(0, _immutableTreasure.Count);
        var so = _immutableTreasure.ToArray()[ranInt];
        return so;
    }

    public void RemovePickedUpTreasure(string coordinates, string _)
    {
        // some other player picked up this treasure
        Debug.Log($"[RemovePickedUpTreasure] " + coordinates);
        Vector3 coords = JsonUtility.FromJson<Vector3>(coordinates); //Vector3Parser.TryParse(coordinates, out Vector3 result) ? result : Vector3.zero;
        PoolSystem.Instance.NetworkedDeSpawn("Treasure", coords);
    }
    
    public void OnTreasurePickedUp(Vector3 origin)
    {
        // remove it for ourselves
        //RemovePickedUpTreasure(JsonUtility.ToJson(origin), "");

        // tell other players to remove it from their worlds
        if (PlayroomKit.IsRunningInBrowser())
        { 
            PlayroomKit.RpcCall("TreasurePickup", origin, PlayroomKit.RpcMode.OTHERS, () => Debug.Log("Treasure de-spawn confirmed"));
        }
        
    }
}
