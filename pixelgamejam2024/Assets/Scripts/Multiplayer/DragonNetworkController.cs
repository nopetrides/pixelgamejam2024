using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Multiplayer;
using Playroom;
using UnityEngine;

/// <summary>
///     Only the host updates the state of the dragon and sets that state for all player.
///     All players read the state and update their dragon appropriately
/// </summary>
public class DragonNetworkController : MonoBehaviour
{
    [SerializeField] private DragonData _dragonStats;

    public Action<DragonData> OnDragonDataRefresh;

    private ConcurrentDictionary<string, int> _currentDragonChange = new();
    private Dictionary<string, int> _networkedDragonData = new();
    private Dictionary<string, int> _networkedDragonOther = new();

    private bool _initialized; // host only, client wait for state
    private const string DragonReady = "DragonReady";

    private GameConstants.DragonStats HeatStat => GameConstants.DragonStats.Heat;
    private GameConstants.DragonStats TemperStat => GameConstants.DragonStats.Temper;
    private GameConstants.DragonStats EnergyStat => GameConstants.DragonStats.Energy;
    private GameConstants.DragonStats ChewingStat => GameConstants.DragonStats.Chewing;

    private void Awake()
    {
        InitializeDragonStats();
        
        if (!PlayroomKit.IsRunningInBrowser() || PlayroomKit.IsHost())
        {
            OnDragonDataInitialized();
            return;
        }

        // Wait for the callback that the host has set up the initial data
        PlayroomKit.WaitForState(DragonReady, WaitForDragonReadyStateCallback);
    }

    private void InitializeDragonStats()
    {
        foreach(var age in _dragonStats.DragonAges)
            age.SetupStats();
        _dragonStats.Health = _dragonStats.MaxHealth;
            
        _currentDragonChange.TryAdd(HeatStat.ToString(), 0);
        _currentDragonChange.TryAdd(TemperStat.ToString(), 0);
        _currentDragonChange.TryAdd(EnergyStat.ToString(), 0);
        _currentDragonChange.TryAdd(ChewingStat.ToString(), 0);
        
        _networkedDragonData.Add(HeatStat.ToString(), 0);
        _networkedDragonData.Add(TemperStat.ToString(), 0);
        _networkedDragonData.Add(EnergyStat.ToString(), 0);
        _networkedDragonData.Add(ChewingStat.ToString(), 0);
        _networkedDragonOther.Add(GameConstants.OtherDragonData.Age.ToString(), 0);
        _networkedDragonOther.Add(GameConstants.OtherDragonData.Growth.ToString(), 0);
        _networkedDragonOther.Add(GameConstants.OtherDragonData.Health.ToString(), 0);
    }

    /// <summary>
    ///     Fixed Update so the dragon stat rises more consistently
    /// </summary>
    private void FixedUpdate()
    {
        if (!_initialized) return;
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
        _networkedDragonOther = PlayroomKit.GetStateDict<int>(nameof(_networkedDragonOther));
        ApplyDragonDataFromNetwork();
        UpdateDragonFiniteStateMachine();
        InformListeners();

        if (_dragonStats.Health <= 0)
        {
            _initialized = false;
            LoadingManager.Instance.LoadScene("GameOverScene");
        }
    }

    private void ApplyDragonDataFromNetwork()
    {
        foreach (var kvp in _networkedDragonData)
        {
            var stat = _dragonStats.CurrentAgeData.CurrentStats[kvp.Key];
            var lastValue = stat.Current;
            var updated = _networkedDragonData[kvp.Key];
            stat.Current = updated;
            stat.ChangeThisFrame = updated - lastValue;
            _dragonStats.CurrentAgeData.CurrentStats[kvp.Key] = stat;
        }

        _dragonStats.Age = _networkedDragonOther[GameConstants.OtherDragonData.Age.ToString()];
        _dragonStats.Growth = _networkedDragonOther[GameConstants.OtherDragonData.Growth.ToString()];
        _dragonStats.Health = _networkedDragonOther[GameConstants.OtherDragonData.Health.ToString()];
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
        OnDragonDataRefresh?.Invoke(_dragonStats);
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
        if (_currentDragonChange.Count != _networkedDragonData.Count)
        {
            Debug.LogError("[SendDragonStatusToNetwork] Mismatched data");
            return;
        }
        
        foreach (var (key, dragonStatus) in _currentDragonChange)
        {
            _networkedDragonData[key] = dragonStatus;
        }

        PlayroomKit.SetState(nameof(_networkedDragonData), _networkedDragonData);

        _networkedDragonOther[GameConstants.OtherDragonData.Age.ToString()] = _dragonStats.Age;
        _networkedDragonOther[GameConstants.OtherDragonData.Growth.ToString()] = _dragonStats.Growth;
        _networkedDragonOther[GameConstants.OtherDragonData.Health.ToString()] = _dragonStats.Health;
        PlayroomKit.SetState(nameof(_networkedDragonOther), _networkedDragonOther, true);
    }
    

    private void CalculateNewDragonState()
    {
        // Check this first on the host to see what modifiers to apply this frame
        UpdateDragonFiniteStateMachine();

        // calculate automatic change based on state
        ModifyStatusBasedOnState();

        // host modifies the authoritative version of the stats
        if (_currentDragonChange[ChewingStat.ToString()] > 0)
        {
            var growth = _currentDragonChange[ChewingStat.ToString()];
            _dragonStats.Growth += growth;
            if (_dragonStats.Growth >= _dragonStats.CurrentAgeData.GrowthRequirement)
            {
                _dragonStats.Age++;
                _dragonStats.Growth = 0;
            }
        }

        foreach (var stat in _currentDragonChange)
        {
            _dragonStats.CurrentAgeData.CurrentStats[stat.Key].ChangeThisFrame = stat.Value;
            int current = _dragonStats.CurrentAgeData.CurrentStats[stat.Key].Current;
            int max = _dragonStats.CurrentAgeData.CurrentStats[stat.Key].Max;
            _dragonStats.CurrentAgeData.CurrentStats[stat.Key].Current = Mathf.Clamp(current + stat.Value, 0, max);
            _currentDragonChange[stat.Key] = 0;
        }
    }

    private void ModifyStatusBasedOnState()
    {
        switch (_dragonAnimationState)
        {
            case FiniteDragonState.Overheated:
                // Tired ++
                // Cranky ++
                // hp --
                _currentDragonChange[EnergyStat.ToString()]++;
                _currentDragonChange[TemperStat.ToString()]++;
                TakeHealthDamage();
                break;
            case FiniteDragonState.Cranky:
                // Tired ++
                // hp --
                _currentDragonChange[EnergyStat.ToString()]++;
                TakeHealthDamage();
                break;
            case FiniteDragonState.Sleeping:
                // Cranky ++
                _currentDragonChange[TemperStat.ToString()]++;
                break;
            case FiniteDragonState.Chewing:
                // Heat ++++
                // Tired ++
                _currentDragonChange[HeatStat.ToString()] += 20;
                _currentDragonChange[EnergyStat.ToString()]++;
                break;
            case FiniteDragonState.Eating:
                // Chewing --
                // Heat ++
                // Cranky --
                _currentDragonChange[ChewingStat.ToString()]--;
                _currentDragonChange[HeatStat.ToString()] += 5;
                _currentDragonChange[TemperStat.ToString()]--;
                break;
            case FiniteDragonState.Idle:
                // Cranky ++
                _currentDragonChange[TemperStat.ToString()]++;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void TakeHealthDamage()
    {
        int hp = _dragonStats.Health;
        hp--;
        _dragonStats.Health = Mathf.Clamp(hp, 0, _dragonStats.MaxHealth);

        if (_dragonStats.Health <= 0)
        {
            LoadingManager.Instance.LoadScene("GameOverScene");
            _initialized = false;
        }
    }
    
    /// <summary>
    ///     Players modifies the dragon's stats by interacting with a dragon station
    ///     Host calculates the final outcome here
    /// </summary>
    /// <param name="stat"></param>
    /// <param name="stationDataAffectValue"></param>
    public bool OnDragonStationUsed(string stat, int stationDataAffectValue)
    {
        if (!_initialized)
        {
            return false;
        }

        //Debug.Log("[OnDragonStationUsed] thead safe?");
        bool hasEffect = stationDataAffectValue > 0 && _dragonStats.CurrentAgeData.CurrentStats[stat].Current > 0;

        // Need to always set the state if that station will effect the dragon
        if (PlayroomKit.IsRunningInBrowser())
            PlayroomKit.SetState(stat, hasEffect, true);

        _currentDragonChange[stat] -= stationDataAffectValue;

        if (!hasEffect) return false;
        
        if (stat != TemperStat.ToString())
        {
            // if not Temper, Cranky ++
            _currentDragonChange[TemperStat.ToString()]++;
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
    public enum FiniteDragonState
    {
        Idle = 0,
        Eating = 1,
        Chewing = 2,
        Sleeping = 3,
        Cranky = 4,
        Overheated = 5,
    }

    private FiniteDragonState _dragonAnimationState = FiniteDragonState.Idle;

    public FiniteDragonState DragonState => _dragonAnimationState;

    private void UpdateDragonFiniteStateMachine()
    {
        if (_dragonAnimationState == FiniteDragonState.Overheated)
        {
            if (!IsStatZeroed(HeatStat)) return;
        }
        if (IsStatMaxed(HeatStat))
        {
            _dragonAnimationState = FiniteDragonState.Overheated;
            return;
        }
        
        if (_dragonAnimationState == FiniteDragonState.Cranky)
        {
            if (!IsStatZeroed(TemperStat)) return;
        }
        if (IsStatMaxed(TemperStat))
        {
            _dragonAnimationState = FiniteDragonState.Cranky;
            return;
        }

        if (_dragonAnimationState == FiniteDragonState.Sleeping)
        {
            if (!IsStatZeroed(EnergyStat)) return;
        }
        if (IsStatMaxed(EnergyStat))
        {
            _dragonAnimationState = FiniteDragonState.Sleeping;
            return;
        }

        if (_dragonAnimationState == FiniteDragonState.Chewing)
        {
            if (!IsStatZeroed(ChewingStat)) return;
        }
        if (IsStatMaxed(ChewingStat))
        {
            _dragonAnimationState = FiniteDragonState.Chewing;
            return;
        }

        if (_dragonStats.CurrentAgeData.CurrentStats[ChewingStat.ToString()].Current > 0)
        {
            _dragonAnimationState = FiniteDragonState.Eating;
            return;
        }

        _dragonAnimationState = FiniteDragonState.Idle;
    }

    /// <summary>
    ///     A stat must return to zero after being maxed
    /// </summary>
    /// <param name="stat"></param>
    /// <returns></returns>
    private bool IsStatZeroed(GameConstants.DragonStats stat)
    {
        return _dragonStats.CurrentAgeData.CurrentStats[stat.ToString()].Current <= 0;
    }
    
    private bool IsStatMaxed(GameConstants.DragonStats stat)
    {
        return _dragonStats.CurrentAgeData.CurrentStats[stat.ToString()].Current >= _dragonStats.CurrentAgeData.CurrentStats[stat.ToString()].Max;
    }

#endregion

    public void DragonEat(int eat)
    {
        if (_currentDragonChange.TryGetValue(ChewingStat.ToString(), out var current))
        {
            _currentDragonChange.TryUpdate(ChewingStat.ToString(), eat, current);
        }
    }
}
