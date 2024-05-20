using System;
using System.Collections;
using System.Collections.Generic;
using Playroom;
//using UnityEditor.Build.Reporting;
using UnityEngine;

public class DragonFeeding : MonoBehaviour
{
    [SerializeField] private DragonNetworkController _dragonNetworkController;
    
    private void Start()
    {
        // all player will call this rpc to ask the server if this treasure spawner was already found
        if (!PlayroomKit.IsRunningInBrowser()) return;
        PlayroomKit.RpcRegister("FeedDragon", ApplyFeedToDragon, "Found Spawner message success");
    }

    /// <summary>
    /// Should have reference to UI to modify the bars according to the treasure values
    /// Detect the player/treasure in the area (big trigger zone) and run the logic accordingly, which is:
    /// is it chewing? then do nothing with the treasure
    /// otherwise, eat the treasure, and build up the chewing
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        //if is chewing return
        int carriedValue = TreasureManager.Instance._localPlayer.DropTreasure();
        Debug.Log($"Drop the Treasure");
        //build up chewing
        if (!PlayroomKit.IsRunningInBrowser())
        {
            ApplyFeedToDragon(PlayroomKit.ConvertToJson(carriedValue), "");
        }
        else
        {
            if (PlayroomKit.IsHost())
                ApplyFeedToDragon(PlayroomKit.ConvertToJson(carriedValue), "");
            else
                PlayroomKit.RpcCall("FeedDragon", carriedValue, () => Debug.Log("Feed confirmed"));
        }
    }

    private void ApplyFeedToDragon(string feedValue, string _)
    {
        int eat = int.Parse(feedValue);
        _dragonNetworkController.DragonEat(eat);
    }
    
}
