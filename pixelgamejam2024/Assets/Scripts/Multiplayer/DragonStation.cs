using System.Collections.Generic;
using HighlightPlus;
using Playroom;
using UnityEngine;
using UnityEngine.Serialization;

public class DragonStation : MonoBehaviour
{
    [FormerlySerializedAs("DragonController")] [Header("Dragon Controller")] [SerializeField]
    private DragonNetworkController _dragonController;

    [FormerlySerializedAs("StationData")] [Header("Station")] [SerializeField]
    private DragonStationSO _stationData;

    [FormerlySerializedAs("GlowHighlight")] [SerializeField]
    private HighlightEffect _glowHighlight;
    
    [SerializeField] private ParticleSystem _activeParticles;

    private List<Collider> _collidersWithinTrigger = new();

    private string StationType => _stationData.AffectsDragonStats.ToString();
    private string StationName => $"DragonStation{_stationData.AffectsDragonStats}";

    private bool _testFlag;

    private bool _initialized;
    
    private void Awake()
    {
        if (PlayroomKit.IsRunningInBrowser())
        {
            /*PlayroomKit.RpcRegister(StationName, 
                OnActivateStationRPC, 
                $"{StationName} Activated!");*/
            PlayroomKit.Me().SetState(StationName, false);
            if (PlayroomKit.IsHost())
            {
                PlayroomKit.SetState(StationType, false, true);
                _initialized = true;
            }
            else
            {
                PlayroomKit.WaitForState(StationType, WaitForStationReadyStateCallback);
            }
        }
        else
        {
            _initialized = true;
        }
    }
    
    private void WaitForStationReadyStateCallback(string callbackOrigin)
    {
        if (callbackOrigin != StationType)
        {
            // Hey, Playroom, this isn't the droid we are looking for. Let's wait AGAIN
            PlayroomKit.WaitForState(StationType, WaitForStationReadyStateCallback);
            return;
        }

        _initialized = true;
    }
    
    private void ApplyStationToDragon(string dataJson, string senderJson)
    {
        //Debug.Log($"[DragonStation] Activating {dataJson}");
        if (PlayroomKit.IsRunningInBrowser())
        {
            if (!PlayroomKit.IsHost())
            {
                Debug.LogError("[DragonStation.OnActivateStationRPC] Only the host can call this!");
                return;
            }
        }
        _testFlag = _dragonController.OnDragonStationUsed(StationType, int.Parse(dataJson));
    }

    private void OnTriggerEnter(Collider other)
    {
        // Enable Interact
        // Debug.Log($"Entered {StationName}");
        if (!_collidersWithinTrigger.Contains(other))
        {
            _collidersWithinTrigger.Add(other);
            if (PlayroomKit.IsRunningInBrowser())
                PlayroomKit.Me().SetState(StationName, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Disable Interact
        // Debug.Log($"Exited {StationName}");
        if (_collidersWithinTrigger.Contains(other))
        {
            _collidersWithinTrigger.Remove(other);
            if (PlayroomKit.IsRunningInBrowser())
                PlayroomKit.Me().SetState(StationName, false);
            else 
                _testFlag = false;
        }
    }

    private void Update()
    {
        if (!_initialized) return;
        
        AffectDragon();

        bool active;
        // Let the host tell us if it is really active
        // it could be on because of other players or off because
        if (PlayroomKit.IsRunningInBrowser())
            active = PlayroomKit.GetStateBool(StationType);
        else
            active = _testFlag;

        _glowHighlight.highlighted = active;
        var emission = _activeParticles.emission;
        emission.enabled = active;
    }

    
    private void AffectDragon()
    {
        if (!PlayroomKit.IsRunningInBrowser())
        {
            ApplyStationToDragon(PlayroomKit.ConvertToJson(_stationData.AffectValue * _collidersWithinTrigger.Count), "");
        }
        else if (PlayroomKit.IsHost())
        {
            ApplyStationToDragon(PlayroomKit.ConvertToJson(_stationData.AffectValue * PlayersInStationRange()), PlayroomKit.Me().id);
        }
    }

    /// <summary>
    /// Host Only
    /// </summary>
    /// <returns></returns>
    private int PlayersInStationRange()
    {
        if (!PlayroomKit.IsHost())
        {
            Debug.LogError("[DragonStation.PlayersInStationRange] Only the host needs to calculate this.");
            return 0;
        }
        var players = PlayroomKit.GetPlayersOrNull();
        int playersInContact = 0;
        foreach (var p in players.Values)
        {
            if (p.GetState<bool>(StationName))
                playersInContact++;
            // TODO get the character type and check if that character has a modifer for this station
        }

        return playersInContact;
    }
    private void AffectDragonRPCConfirm()
    {
        Debug.Log("Dragon Station RPC confirmed");
    }

}
