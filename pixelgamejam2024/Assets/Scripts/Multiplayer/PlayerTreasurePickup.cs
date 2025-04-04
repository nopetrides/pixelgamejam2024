using CMF;
using Multiplayer;
using Playroom;
using UnityEngine;

public class PlayerTreasurePickup : MonoBehaviour
{
    private AdvancedWalkerController _controller;
    
    private int _baseWeight;
    private int _carriedWeight = 0;
    [SerializeField]
    private int _carryLimit = 60;
    [SerializeField]
    private int _carryThreshold;
    
    private void Awake()
    {
        _carriedWeight = _baseWeight;
        _controller = GetComponent<AdvancedWalkerController>();
    }
    
    public void AddToWeight(int weight, Vector3 location)
    {
        _carriedWeight += weight;
        if (_carriedWeight >= _carryLimit) _carriedWeight = _carryLimit;
        if (_carriedWeight < 0) _carriedWeight = 0;
        //Debug.Log($"Carrying: {_carriedWeight.ToString()}");
        //Debug.Log($"Speed mod: {(1 - (float)_carriedWeight/(float)_carryLimit) * 2}");
        if (_carriedWeight < _carryThreshold) return;
        _controller.SetMoveSpeed((1 - (float)_carriedWeight/(float)_carryLimit) * 2);
        if (PlayroomKit.IsRunningInBrowser()) PlayroomKit.Me().SetState(GameConstants.PlayerStateData.IsCarrying.ToString(), true);
    }
    
    public int GetWeight() => _carriedWeight;
    public int GetCarryLimit() => _carryLimit;
    public void SetLimitAndThreshold(int capacity)
    {
        _carryLimit = capacity;
        _carryThreshold = _carryLimit / 2;
    }

    public int DropTreasure()
    {
        /*foreach (var VARIABLE in _pickedUpTreasureCoordinates)
        {
            PoolSystem.Instance.Spawn("Treasure", VARIABLE);
        }*/
        int carried = _carriedWeight;
        _carriedWeight = _baseWeight;
        _controller.SetMoveSpeed(1);
        if (PlayroomKit.IsRunningInBrowser()) PlayroomKit.Me().SetState(GameConstants.PlayerStateData.IsCarrying.ToString(), false);
        Debug.Log($"Treasure Dropped");
        return carried;
    }

    public bool IsCarrying() => _carriedWeight >= _carryThreshold;

    private void Update()
    {
        //if(Input.GetKeyDown(KeyCode.Alpha1)) Debug.Log($"Speed mod: {1 - (float)_carryThreshold/(float)_carryLimit}");
    }
}
