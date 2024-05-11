using System.Collections;
using System.Collections.Generic;
using Playroom;
using UnityEngine;

public class PlayerNetworkController : MonoBehaviour
{
    private PlayroomKit.Player _localPlayerObject;
    
    // Start is called before the first frame update
    void Start()
    {
        _localPlayerObject = PlayroomKit.MyPlayer();
        if (_localPlayerObject != null)
            Debug.Log($"[PlayerNetworkController] started with player {_localPlayerObject.id}");
        else
            Debug.LogError("[PlayerNetworkController] no Playroom Player found");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
