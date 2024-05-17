using System;
using System.Collections.Generic;
using Playroom;
using UnityEngine;
using Random = UnityEngine.Random;

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

    private Rigidbody _localPlayerRigidbody;

    [SerializeField]
    private List<TreasureTypesSO> _treasureTypes;

    [SerializeField]
    private List<string> _treasureNames;

    

    /// <summary>
    /// Each treasure spawner has a unique id so we can track
    /// that unique id is based on their world coordinates which will be the same between players
    /// regardless of the order they uncover the map
    /// </summary>
    private Dictionary<Vector3, string> TreasureSpawner = new();

    /// <summary>
    /// Treasure Object Pool
    /// todo
    /// </summary>
    /// <returns></returns>
    Dictionary<string, TreasureDataSerializer> TreasureTracker = new();
    
    public void SetLocalPlayerRigidbody(Rigidbody playerRb)
    {
        _localPlayerRigidbody = playerRb;
    }
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        Debug.Log(_treasureTypes.Count);
        
        // all player will call this rpc to ask the server if this treasure spawner was already found
        if (!PlayroomKit.IsRunningInBrowser()) return;
        PlayroomKit.RpcRegister("FoundTreasureSpawner", DoesTreasureSpawnerExistInDictionary, "Found Spawner message success");
        // Only the host will call this rpc to tell all players to spawn a new treasure object at a location.
        PlayroomKit.RpcRegister("SpawnTreasureEveryone", AddNewTreasureToDictionary, "Treasure created confirmed");
    }

    /// <summary>
    ///     Comes from the map node, all players will call this
    /// </summary>
    /// <param name="TreasureSpawnerCoordinates"></param>
    public void AskServerIfTreasureSpawnerWasAlreadyFound(string TreasureSpawnerCoordinates)
    {
        if (!PlayroomKit.IsRunningInBrowser())
        {
            DoesTreasureSpawnerExistInDictionary(TreasureSpawnerCoordinates, "ID");
        }
        else
        {
            PlayroomKit.RpcCall("FoundTreasureSpawner", TreasureSpawnerCoordinates, PlayroomKit.RpcMode.HOST, () => Debug.Log("Treasure spawner check confirmed"));
        }
        
    }

    public void DoesTreasureSpawnerExistInDictionary(string coordinates, string _)
    {
        // check dictionary
        Vector3 coords = Vector3Parser.TryParse(coordinates, out Vector3 result) ? result : Vector3.zero;
        bool inDictionary = TreasureSpawner.ContainsKey(coords);
        if (inDictionary) return;

        // if does not yet exist:
        // tell all player to spawn this thing, and add it to their local dictionary
        var spawnerID = "Spawner_ID_" + coordinates;
        TreasureSpawner.Add(coords, spawnerID);
        
        //logic to randomize treasure before spawning
        
        var treasureData = _treasureTypes[Random.Range(0, _treasureTypes.Count)];
        Debug.Log($"{treasureData.Type}");
        var messageData = new TreasureDataSerializer(){Coordinates = coords, Type = treasureData.Type};
        string serializedTreasureData = JsonUtility.ToJson(messageData);
        
        
        if (!PlayroomKit.IsRunningInBrowser())
        {
            AddNewTreasureToDictionary(serializedTreasureData, "");
        }
        else
        {
            if (PlayroomKit.IsHost()) PlayroomKit.RpcCall("SpawnTreasureEveryone", serializedTreasureData, PlayroomKit.RpcMode.ALL, () => Debug.Log("Treasure spawn confirmed"));
        }
    }
    
    private void AddNewTreasureToDictionary(string data, string _)
    {
        // Server has told us to make a new treasure in the world.
        // add to to the object pool
        // get next pooled object and show it
        // todo, coordinates where the new treasure item spawns
        
        var unparse = JsonUtility.FromJson<TreasureDataSerializer>(data);
        PoolSystem.Instance.Spawn("Treasure", unparse.Coordinates, data);

    }

    public TreasureTypesSO Deserializer(string data)
    {
        var treasureData = JsonUtility.FromJson<TreasureDataSerializer>(data);
        var index = _treasureNames.IndexOf(treasureData.Type);
        return _treasureTypes[index];
    }

    
    [Serializable]
    private class TreasureDataSerializer
    {
        public Vector3 Coordinates;
        public string Type;
    }
}
