using System.Collections.Generic;
using HighlightPlus;
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

    private List<Collider> _collidersWithinTrigger = new();

    /*
    private void Awake()
    {
        if (PlayroomKit.IsRunningInBrowser())
        {
            PlayroomKit.RpcRegister($"DragonStation{_stationData.AffectsDragonStats}", 
                OnActivateStationRPC, 
                $"Station {_stationData.AffectsDragonStats} Activated!");
        }
    }
    
    private void OnActivateStationRPC(string dataJson, string senderJson)
    {
        _dragonController.OnDragonStationUsed(dataJson, _stationData.AffectValue * _collidersWithinTrigger.Count);
    }
*/

    private void OnTriggerEnter(Collider other)
    {
        // Enable Interact
        Debug.Log($"Entered Station {_stationData.AffectsDragonStats.ToString()}");
        if (!_collidersWithinTrigger.Contains(other))
            _collidersWithinTrigger.Add(other);

        _glowHighlight.highlighted = true;
    }

    private void OnTriggerExit(Collider other)
    {
        // Disable Interact
        Debug.Log($"Exited Station {_stationData.AffectsDragonStats.ToString()}");
        if (_collidersWithinTrigger.Contains(other))
            _collidersWithinTrigger.Remove(other);

        _glowHighlight.highlighted = false;
    }

    private void Update()
    {
        _glowHighlight.highlighted = _collidersWithinTrigger.Count > 0;

        if (_glowHighlight.highlighted)
            _dragonController.OnDragonStationUsed(_stationData.AffectsDragonStats.ToString(),
                _stationData.AffectValue * _collidersWithinTrigger.Count);
        //AffectDragon();
    }

    /*
    private void AffectDragon()
    {
        if (!PlayroomKit.IsRunningInBrowser())
        {
            OnActivateStationRPC(_stationData.AffectsDragonStats.ToString(), "");
            return;
        }
        if (PlayroomKit.IsHost())
        {
            OnActivateStationRPC(_stationData.AffectsDragonStats.ToString(), PlayroomKit.Me().id);
        }
        else
        {
            PlayroomKit.RpcCall($"DragonStation{_stationData.AffectsDragonStats}", _stationData.AffectsDragonStats.ToString(), PlayroomKit.RpcMode.HOST,
                AffectDragonRPCConfirm);
        }
    }
    private void AffectDragonRPCConfirm()
    {
        Debug.Log("Dragon Station RPC confirmed");
    }
*/
}
