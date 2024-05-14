using System.Collections.Generic;
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
    
    private void Awake()
    {
    }
}
