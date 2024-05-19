using System.Collections;
using System.Collections.Generic;
using Multiplayer;
using Playroom;
using UnityEngine;

public class PlayerTreasurePickup : MonoBehaviour
{
    private int _weight;
    private int _threshold;
    private void Awake()
    {
        TreasureManager.Instance.SetLocalPlayerPickup(this);
    }
    
    public void AddToWeight(int weight)
    {
        _weight += weight; //Switch to the 
        Debug.Log($"{_weight}");
        if(PlayroomKit.IsRunningInBrowser() && _weight > _threshold) PlayroomKit.Me().SetState(GameConstants.PlayerStateData.IsCarrying.ToString(), true);
    }

    private void OnTriggerEnter(Collider other)
    {
        //if it is a treasure in the manager dictionary, despawn for all, add it to own inventory
        //int weightChange = TreasureManager.Instance.IsTreasureInDictionary(other.)
        
        //if dragon zone, drop the treasure
    }
}
