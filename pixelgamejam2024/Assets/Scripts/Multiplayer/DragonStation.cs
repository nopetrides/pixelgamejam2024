using HighlightPlus;
using Playroom;
using UnityEngine;

public class DragonStation : MonoBehaviour
{
    [Header("Dragon Controller")] 
    [SerializeField] private DragonNetworkController DragonController;
    [Header("Station")]
    [SerializeField] private DragonStationSO StationData;
    [SerializeField] private HighlightEffect GlowHighlight;

    private void Awake()
    {
        if (PlayroomKit.IsRunningInBrowser())
        {
            PlayroomKit.RpcRegister($"DragonStation{StationData.AffectsDragonStats}", OnActivateStationRPC, $"Station {StationData} Activated!");
        }
    }

    private void OnActivateStationRPC(string dataJson, string senderJson)
    {
        DragonController.OnDragonStationUsed(dataJson, StationData.AffectValue);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Enable Interact
        Debug.Log($"Entered Station {StationData.AffectsDragonStats.ToString()}");
        GlowHighlight.highlighted = true;
    }

    private void OnTriggerExit(Collider other)
    {
        // Disable Interact
        Debug.Log($"Exited Station {StationData.AffectsDragonStats.ToString()}");
        GlowHighlight.highlighted = false;
    }

    private void Update()
    {
        if (GlowHighlight.highlighted)
            AffectDragon();
    }

    private void AffectDragon()
    {
        if (!PlayroomKit.IsRunningInBrowser())
        {
            OnActivateStationRPC(StationData.AffectsDragonStats.ToString(), "");
            return;
        }
        if (PlayroomKit.IsHost())
        {
            OnActivateStationRPC(StationData.AffectsDragonStats.ToString(), PlayroomKit.Me().id);
        }
        else
        {
            PlayroomKit.RpcCall($"DragonStation{StationData.AffectsDragonStats}", StationData.AffectsDragonStats.ToString(), PlayroomKit.RpcMode.HOST,
                AffectDragonRPCConfirm);
        }
    }

    private void AffectDragonRPCConfirm()
    {
        Debug.Log("Dragon Station RPC confirmed");
    }
}
