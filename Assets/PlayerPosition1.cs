using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerPosition1 : NetworkBehaviour
{

    public static NetworkVariable<Vector3> player1Position = new NetworkVariable<Vector3>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public static NetworkVariable<Vector3> player2Position = new NetworkVariable<Vector3>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public static Vector3 midpoint;


    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void OnNetworkSpawn()
    {
       if( IsHost)
        {
            player1Position.Value = transform.position;
        }
        else if( IsClient)
        {
            ServerClientPositionServerRpc();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;


        if (IsHost)
        {
            player1Position.Value = transform.position;
        }
        else if (IsClient)
        {
            ServerClientPositionServerRpc();
        }

        midpoint = (player1Position.Value + player2Position.Value) / 2;
    }


    private void OnGUI()
    {
        // Display host and client positions in the UI
        if (IsServer)
        {
            GUI.Label(new Rect(10, 10, 200, 20), "Host Position: " + player1Position.Value);
        }

        if (IsClient)
        {
            GUI.Label(new Rect(10, 80, 200, 20), "Client Position: " + player2Position.Value);
        }
    }


    [ServerRpc]
    public void ServerClientPositionServerRpc()
    {
        ClientPositionClientRpc();
    }

    [ClientRpc]
    public void ClientPositionClientRpc()
    {
        player2Position.Value = transform.position;
    }

    [ServerRpc]
    public void ServerHostPositionServerRpc()
    {
        player1Position.Value = transform.position;
    }

    [ClientRpc]
    public void HostPositionClientRpc()
    {
        player1Position.Value = transform.position;
    }

}
