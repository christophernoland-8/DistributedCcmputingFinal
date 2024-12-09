using Unity.Netcode;
using UnityEngine;

public class CollectibleItem : NetworkBehaviour
{
    [SerializeField] private Transform buoy;
    private Transform buoyPrefab;

    private void Update()
    {
        if(TimerUI.remainTime.Value == 0)
        {
            NetworkObject.Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if(IsServer)
            {
                NetworkObject.Destroy(gameObject);


                BuoySpawner.isSpawned = false;
            }
        }
    }

}
