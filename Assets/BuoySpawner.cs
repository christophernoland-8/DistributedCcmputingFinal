using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class BuoySpawner : NetworkBehaviour
{
    [SerializeField] private Transform buoy;
    private Transform buoyPrefab;

    

    public static bool isSpawned = true;

    //int clientId = ServerRpcParams.Receive.SenderClientId;

    // Start is called before the first frame update
    void Start()
    {
        if (NetworkManager.Singleton.IsServer && IsOwner)
        {
            buoyPrefab = Instantiate(buoy);
            buoyPrefab.GetComponent<NetworkObject>().Spawn(true);
            // If this is the local player's object, find the other player's object.
            // This assumes you have a reference to the other player's NetworkObject.
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        Vector3 vector3 = new Vector3(5.0f, 0, 5.0f);

        Vector3 testPoint = PlayerPosition1.midpoint + vector3;

        if (Input.GetKeyDown(KeyCode.Space) && IsOwner)
        {
            buoyPrefab = Instantiate(buoy, testPoint, Quaternion.identity);
            buoyPrefab.GetComponent<NetworkObject>().Spawn(true);
        }

        if(isSpawned == false)
        {
            spawnBuoy();
        }
    }


    public void spawnBuoy()
    {
        Vector3 vector3 = new Vector3(5.0f, 0, -5.0f);

        Vector3 testPoint = PlayerPosition1.midpoint + vector3;

        buoyPrefab = Instantiate(buoy, PlayerPosition1.midpoint, Quaternion.identity);
        buoyPrefab.GetComponent<NetworkObject>().Spawn(true);

        isSpawned = true;
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Coin" && IsServer && IsOwner)
        {
            ScoreManager.scoreTest1.Value++;
        }
        else if(other.tag == "Coin")
        {
            ScoreManager.scoreTest2.Value++;
        }

    }
}
