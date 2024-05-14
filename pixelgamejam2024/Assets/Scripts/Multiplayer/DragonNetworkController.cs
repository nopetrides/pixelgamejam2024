using System;
using System.Collections.Generic;
using Playroom;
using UnityEngine;


/// <summary>
///     Only the host updates the state of the dragon and sets that state for all player.
///     All players read the state and update their dragon appropriately
/// </summary>
public class DragonNetworkController : MonoBehaviour
{
    public Action<Dictionary<string, int>> OnDragonDataRefresh;
    
    private Dictionary<string, int> DragonData = new ();

    private bool _initialized;

    // Update is called once per frame
    private void Update()
    {
        if (!PlayroomKit.IsRunningInBrowser())
            MockDragon();
        else
            CheckDragonState();
    }

    private void MockDragon()
    {
        CalculateNewDragonState();
        InformListeners();
    }

    private void CheckDragonState()
    {
        if (PlayroomKit.IsHost())
            SetDragonState();
        else
            GetDragonState();
    }
    
    #region Host
    private void SetDragonState()
    {
        CalculateNewDragonState();
        PlayroomKit.SetState(nameof(DragonData), DragonData);
        InformListeners();
    }

    // todo move to a better spot?
    private void CalculateNewDragonState()
    {
        InitComplete();
        string[] statNames = {"Temper", "Heat", "Chewing", "Energy"}; // for testing
        foreach(var stat in statNames)
        {
            if (!DragonData.ContainsKey(stat))
            {
                DragonData.Add(stat, 0);
            }

            DragonData[stat] = Time.frameCount;
        }
    }
    #endregion

    private void GetDragonState()
    {
        if (!_initialized)
        {
            PlayroomKit.WaitForState(nameof(DragonData), InitComplete);
            return;
        }
        DragonData = PlayroomKit.GetStateDict<int>(nameof(DragonData));
        InformListeners();
    }

    private void InitComplete()
    {
        _initialized = true;
    }

    private void InformListeners()
    {
        OnDragonDataRefresh?.Invoke(DragonData);
    }
}
