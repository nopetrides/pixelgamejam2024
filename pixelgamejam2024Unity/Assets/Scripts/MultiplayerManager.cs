using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Playroom;

public class MultiplayerManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var initOptions = new PlayroomKit.InitOptions()
        {
            maxPlayersPerRoom = 4,
            defaultPlayerStates = new()
            {
                {"pos", Vector3.zero}
            }
        };
        
        Playroom.PlayroomKit.InsertCoin(initOptions, OnPlayerStartLobby, OnPlayerDisconnect);
    }

    void OnPlayerStartLobby()
    {
        
    }

    void OnPlayerDisconnect()
    {
        
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
