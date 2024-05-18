using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Multiplayer;
using Playroom;
using UnityEngine;


/// <summary>
///     Only the host updates the state of the dragon and sets that state for all player.
///     All players read the state and update their dragon appropriately
/// </summary>
public class DragonNetworkController : MonoBehaviour
{
    [Serializable]
    public class DragonStatus
    {
        public GameConstants.DragonStats Stat;
        public int Max;
        [NonSerialized] public int Current;
        [NonSerialized] public int ChangeThisFrame;
    }

    [SerializeField] private DragonStatus[] DragonStats;

    public Action<ConcurrentDictionary<string, DragonStatus>> OnDragonDataRefresh;

    private ConcurrentDictionary<string, DragonStatus> _currentDragonStatus = new();
    private Dictionary<string, int> _networkedDragonData = new();

    private long _timestampGameStart;

    private bool _initialized; // host only, client wait for state
    private const string DragonReady = "DragonReady";

    private GameConstants.DragonStats HeatStat => GameConstants.DragonStats.Heat;
    private GameConstants.DragonStats TemperStat => GameConstants.DragonStats.Temper;
    private GameConstants.DragonStats EnergyStat => GameConstants.DragonStats.Energy;
    private GameConstants.DragonStats ChewingStat => GameConstants.DragonStats.Chewing;

    private void Awake()
    {
        _timestampGameStart = DateTime.Now.Ticks;
        InitializeDragonStatusDict();
        
        if (!PlayroomKit.IsRunningInBrowser() || PlayroomKit.IsHost())
        {
            OnDragonDataInitialized();
            return;
        }

        // Wait for the callback that the host has set up the initial data
        PlayroomKit.WaitForState(DragonReady, WaitForDragonReadyStateCallback);
    }

    private void InitializeDragonStatusDict()
    {
        var heatStat = DragonStats.FirstOrDefault(stat => stat.Stat == HeatStat);
        var temperStat = DragonStats.FirstOrDefault(stat => stat.Stat == TemperStat);
        var energyStat = DragonStats.FirstOrDefault(stat => stat.Stat == EnergyStat);
        var chewingStat = DragonStats.FirstOrDefault(stat => stat.Stat == ChewingStat);

        _currentDragonStatus.TryAdd(HeatStat.ToString(), heatStat);
        _currentDragonStatus.TryAdd(TemperStat.ToString(), temperStat);
        _currentDragonStatus.TryAdd(EnergyStat.ToString(), energyStat);
        _currentDragonStatus.TryAdd(ChewingStat.ToString(), chewingStat);
        
        _networkedDragonData.Add(HeatStat.ToString(), 0);
        _networkedDragonData.Add(TemperStat.ToString(), 0);
        _networkedDragonData.Add(EnergyStat.ToString(), 0);
        _networkedDragonData.Add(ChewingStat.ToString(), 0);
    }

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
    
    private void InitComplete()
    {
        Debug.Log("Init Complete");
        _initialized = true;
    }

#region Clients

    private void GetDragonState()
    {
        if (!_initialized)
        {
            return;
        }

        _networkedDragonData = PlayroomKit.GetStateDict<int>(nameof(_networkedDragonData));
        ApplyDragonDataFromNetwork();
        UpdateDragonFiniteStateMachine();
        InformListeners();
    }

    private void ApplyDragonDataFromNetwork()
    {
        if (_currentDragonStatus.Count != _networkedDragonData.Count)
        {
            Debug.LogError("[ApplyDragonDataFromNetwork] Mismatched data");
            return;
        }

        foreach (var kvp in _currentDragonStatus)
        {
            var original = _currentDragonStatus[kvp.Key];
            var copy = _currentDragonStatus[kvp.Key];
            var lastValue = _currentDragonStatus[kvp.Key].Current;
            var updated = _networkedDragonData[kvp.Key];
            copy.Current = updated;
            copy.ChangeThisFrame = updated - lastValue;
            _currentDragonStatus.TryUpdate(kvp.Key, copy, original);
        }
    }

    private void WaitForDragonReadyStateCallback(string callbackOrigin)
    {
        if (callbackOrigin != DragonReady)
        {
            // Hey, Playroom, this isn't the droid we are looking for. Let's wait AGAIN
            PlayroomKit.WaitForState(DragonReady, WaitForDragonReadyStateCallback);
            return;
        }
        OnDragonReady();
    }

    private void OnDragonReady()
    {
        Debug.Log("OnDragonReady");
        InitComplete();
    }


    private void InformListeners()
    {
        OnDragonDataRefresh?.Invoke(_currentDragonStatus);
    }

#endregion

#region Host

    private void OnDragonDataInitialized()
    {
        Debug.Log("Host: OnDragonDataInitialized Enter");
        bool host = PlayroomKit.IsRunningInBrowser() && PlayroomKit.IsHost();
        if (host)
            SendDragonStatusToNetwork();
        InitComplete();
        if (host)
            PlayroomKit.SetState(DragonReady, true, true);
        Debug.Log("Host: OnDragonDataInitialized Exit");
    }

    private void SetDragonState()
    {
        CalculateNewDragonState();
        SendDragonStatusToNetwork();
        InformListeners();
    }

    private void SendDragonStatusToNetwork()
    {
        if (_currentDragonStatus.Count != _networkedDragonData.Count)
        {
            Debug.LogError("[SendDragonStatusToNetwork] Mismatched data");
            return;
        }
        
        foreach (var (key, dragonStatus) in _currentDragonStatus)
        {
            _networkedDragonData[key] = dragonStatus.Current;
        }

        PlayroomKit.SetState(nameof(_networkedDragonData), _networkedDragonData, true);
        
    }
    

    private void CalculateNewDragonState()
    {
        // Check this first on the host to see what modifiers to apply this frame
        UpdateDragonFiniteStateMachine();

        // calculate automatic change based on state
        ModifyStatusBasedOnState();

        foreach (var stat in _currentDragonStatus)
        {
            int change = stat.Value.ChangeThisFrame;
            stat.Value.Current = Mathf.Clamp(stat.Value.Current + change, 0, stat.Value.Max);
            stat.Value.ChangeThisFrame = 0;
        }
    }

    private void ModifyStatusBasedOnState()
    {
        switch (_dragonAnimationState)
        {
            case FiniteDragonState.Overheated:
                // Tired ++
                // Cranky ++
                _currentDragonStatus[EnergyStat.ToString()].ChangeThisFrame++;
                _currentDragonStatus[TemperStat.ToString()].ChangeThisFrame++;
                break;
            case FiniteDragonState.Cranky:
                // Tired ++
                _currentDragonStatus[EnergyStat.ToString()].ChangeThisFrame++;
                break;
            case FiniteDragonState.Sleeping:
                // Cranky ++
                _currentDragonStatus[TemperStat.ToString()].ChangeThisFrame++;
                break;
            case FiniteDragonState.Chewing:
                // Heat ++
                // Tired ++
                _currentDragonStatus[TemperStat.ToString()].ChangeThisFrame++;
                _currentDragonStatus[EnergyStat.ToString()].ChangeThisFrame++;
                break;
            case FiniteDragonState.Eating:
                // Chewing ++
                _currentDragonStatus[ChewingStat.ToString()].ChangeThisFrame++;
                break;
            case FiniteDragonState.Idle:
                // Cranky ++
                _currentDragonStatus[TemperStat.ToString()].ChangeThisFrame++;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    
    /// <summary>
    ///     Player modifies the dragon by interacting with a dragon station
    /// </summary>
    /// <param name="stat"></param>
    /// <param name="stationDataAffectValue"></param>
    public bool OnDragonStationUsed(string stat, int stationDataAffectValue)
    {
        if (!_initialized)
        {
            return false;
        }

        bool hasEffect = stationDataAffectValue > 0 && _currentDragonStatus[stat].Current > 0;

        // Need to always set the state if that station will have an effect on the dragon
        if (PlayroomKit.IsRunningInBrowser()) 
            PlayroomKit.SetState(stat, hasEffect, true);
            
        
        if (!hasEffect) return false;

        _currentDragonStatus[stat].ChangeThisFrame -= stationDataAffectValue;

        if (stat != TemperStat.ToString())
        {
            // if not Temper, Cranky ++
            _currentDragonStatus[TemperStat.ToString()].ChangeThisFrame++;
        }

        return true;
    }


#endregion

#region Dragon Finite State Machine

    /// <summary>
    ///     Priority:
    ///         Lowest
    ///         Highest
    ///     e.g. if the dragon can eat, it will, otherwise it is idle
    /// </summary>
    private enum FiniteDragonState
    {
        Idle = 0,
        Eating = 1,
        Chewing = 2,
        Sleeping = 3,
        Cranky = 4,
        Overheated = 5,
    }

    private FiniteDragonState _dragonAnimationState = FiniteDragonState.Idle;

    public string DragonStateDebug => _dragonAnimationState.ToString();

    private void UpdateDragonFiniteStateMachine()
    {
        if (IsStatMaxed(HeatStat))
        {
            _dragonAnimationState = FiniteDragonState.Overheated;
            return;
        }

        if (IsStatMaxed(GameConstants.DragonStats.Temper))
        {
            _dragonAnimationState = FiniteDragonState.Cranky;
            return;
        }

        if (IsStatMaxed(GameConstants.DragonStats.Energy))
        {
            _dragonAnimationState = FiniteDragonState.Sleeping;
            return;
        }

        if (IsStatMaxed(GameConstants.DragonStats.Chewing))
        {
            _dragonAnimationState = FiniteDragonState.Chewing;
            return;
        }

        if (false) // todo food check
        {
            _dragonAnimationState = FiniteDragonState.Eating;
            return;
        }

        _dragonAnimationState = FiniteDragonState.Idle;
    }

    private bool IsStatMaxed(GameConstants.DragonStats stat)
    {
        return _currentDragonStatus[stat.ToString()].Current >= _currentDragonStatus[stat.ToString()].Max;
    }

    private void OnDragonEat()
    {
        
    }

    private void OnDragonIdle()
    {
        
    }

    private void OnTiredMax()
    {
        // Enter Sleep State
        // Won't eat
    }

    private void OnHeatMax()
    {
        // Enter Flaming State
        // Won't eat
    }

    private void OnChewingMax()
    {
        // Heat Rapidly Increases
        // Won't eat
    }

    private void OnCrankyMax()
    {
        // Spills treasure?
        // Won't eat
    }

#endregion

}
