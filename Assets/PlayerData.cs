using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Unity.Collections;
using System;


public class PlayerData : NetworkBehaviour
{
    

    public NetworkVariable<int> Score = new NetworkVariable<int>();
    public NetworkVariable<FixedString128Bytes> Name = new NetworkVariable<FixedString128Bytes>();
    public string playerName = PlayerName.playerName;

    // Start is called before the first frame update
    void Start()
    {
        //var clientID = SenderClientId;
        var test = GetComponent<NetworkObject>().OwnerClientId;

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
