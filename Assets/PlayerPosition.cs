using Unity.Netcode;
using UnityEngine;

public class PlayerDistance : NetworkBehaviour
{
    private Transform otherPlayerTransform;

    // Optionally, you can use OnNetworkSpawn() to get the other player's position
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            // If this is the local player's object, find the other player's object.
            // This assumes you have a reference to the other player's NetworkObject.
            FindOtherPlayer();
        }
    }

    void Update()
    {
        if (IsOwner && otherPlayerTransform != null)
        {
            // Calculate distance between the local player and the other player
            float distance = Vector3.Distance(transform.position, otherPlayerTransform.position);
            Debug.Log("Distance between players: " + distance);
        }
    }

    // Find the other player object in the scene
    private void FindOtherPlayer()
    {
        // Here, we assume there's only one other player. You can modify this for a larger number of players.
        NetworkObject[] networkObjects = FindObjectsOfType<NetworkObject>();
        foreach (var netObject in networkObjects)
        {
            // Skip this player object (we don't want to find the local player's own object)
            if (netObject.IsOwner) continue;

            // Otherwise, this is the other player. We get their transform.
            otherPlayerTransform = netObject.transform;
            break;  // Assuming there's only one other player; adapt if needed.
        }
    }
}
