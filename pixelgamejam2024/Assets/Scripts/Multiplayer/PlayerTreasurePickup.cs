using System;
using System.Collections;
using System.Collections.Generic;
using CMF;
using Multiplayer;
using Playroom;
using TMPro;
using UnityEngine;

public class PlayerTreasurePickup : MonoBehaviour
{
    private AdvancedWalkerController _controller;
    
    private int _baseWeight;
    private int _carriedWeight = 0;
    [SerializeField]
    private int _threshold = 30;
    private List<Vector3> _pickedUpTreasureCoordinates = new();

    [SerializeField]
    private TMP_Text _weightText;
    
    private void Awake()
    {
        TreasureManager.Instance.SetLocalPlayerPickup(this);
        _carriedWeight = _baseWeight;
        _controller = GetComponent<AdvancedWalkerController>();
    }
    
    public void AddToWeight(int weight, Vector3 location)
    {
        _carriedWeight += weight;
        if (_carriedWeight >= _threshold) _carriedWeight = _threshold;
        if (_carriedWeight < 0) _carriedWeight = 0;
        _pickedUpTreasureCoordinates.Add(location);
        Debug.Log($"{_carriedWeight.ToString()}");
        _controller.SetMoveSpeed(1 - ((float)_carriedWeight/(float)_threshold));
        if(PlayroomKit.IsRunningInBrowser() && _carriedWeight > _threshold) PlayroomKit.Me().SetState(GameConstants.PlayerStateData.IsCarrying.ToString(), true);
    }
    
    public int GetWeight() => _carriedWeight;
    public int GetThreshold() => _threshold;
    public void SetThreshold(int capacity) => _threshold = capacity;
    public void DropTreasure()
    {
        foreach (var VARIABLE in _pickedUpTreasureCoordinates)
        {
            PoolSystem.Instance.Spawn("Treasure", VARIABLE);
        }
        _pickedUpTreasureCoordinates.Clear();
        _carriedWeight = _baseWeight;
        _controller.SetMoveSpeed(1);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1)) DropTreasure();
        _weightText.text = _carriedWeight.ToString();
    }
}
