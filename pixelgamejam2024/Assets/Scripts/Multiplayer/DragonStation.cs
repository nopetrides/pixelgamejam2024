using HighlightPlus;
using Playroom;
using UnityEngine;
using UnityEngine.Serialization;

public class DragonStation : MonoBehaviour
{
    [FormerlySerializedAs("DragonController")]
    [Header("Dragon Controller")] 
    [SerializeField] private DragonNetworkController _dragonController;
    [FormerlySerializedAs("StationData")]
    [Header("Station")]
    [SerializeField] private DragonStationSO _stationData;
    [FormerlySerializedAs("GlowHighlight")] 
    [SerializeField] private HighlightEffect _glowHighlight;

    private void Awake()
    {
        if (PlayroomKit.IsRunningInBrowser())
        {
            PlayroomKit.RpcRegister($"DragonStation{_stationData.AffectsDragonStats}", 
                OnActivateStationRPC, 
                $"Station {_stationData} Activated!");
        }
    }

    private void OnActivateStationRPC(string dataJson, string senderJson)
    {
        _dragonController.OnDragonStationUsed(dataJson, _stationData.AffectValue);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Enable Interact
        Debug.Log($"Entered Station {_stationData.AffectsDragonStats.ToString()}");
        _glowHighlight.highlighted = true;
    }

    private void OnTriggerExit(Collider other)
    {
        // Disable Interact
        Debug.Log($"Exited Station {_stationData.AffectsDragonStats.ToString()}");
        _glowHighlight.highlighted = false;
    }

    private void Update()
    {
        if (_glowHighlight.highlighted)
            AffectDragon();
    }

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
}
