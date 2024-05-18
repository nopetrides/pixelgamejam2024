using System.Collections.Generic;
using HighlightPlus;
using Playroom;
using Unity.VisualScripting;
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
    
    private void Awake()
    {
        if (PlayroomKit.IsRunningInBrowser())
        {
            PlayroomKit.RpcRegister(StationName, 
                OnActivateStationRPC, 
                $"{StationName} Activated!");
            if (PlayroomKit.IsHost()) PlayroomKit.SetState(StationType, false, true);
        }
    }
    
    private void OnActivateStationRPC(string dataJson, string senderJson)
    {
        Debug.Log($"[DragonStation] Activating {dataJson}");
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
        Debug.Log($"Entered {StationName}");
        if (!_collidersWithinTrigger.Contains(other))
            _collidersWithinTrigger.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        // Disable Interact
        Debug.Log($"Exited {StationName}");
        if (_collidersWithinTrigger.Contains(other))
            _collidersWithinTrigger.Remove(other);
    }

    private void Update()
    {
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
            OnActivateStationRPC(PlayroomKit.ConvertToJson(_stationData.AffectValue * _collidersWithinTrigger.Count), "");
            return;
        }
        if (PlayroomKit.IsHost())
        {
            OnActivateStationRPC(PlayroomKit.ConvertToJson(_stationData.AffectValue * _collidersWithinTrigger.Count), PlayroomKit.Me().id);
        }
        else
        {
            PlayroomKit.RpcCall(StationName,_stationData.AffectValue * _collidersWithinTrigger.Count, PlayroomKit.RpcMode.HOST,
                AffectDragonRPCConfirm);
        }
    }
    private void AffectDragonRPCConfirm()
    {
        Debug.Log("Dragon Station RPC confirmed");
    }

}
