using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PointCollect : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && IsHost)
        {
            ScoreManager.scoreTest1.Value += 0;
        }
        else if(other.gameObject.tag == "Player" && IsClient)
        {
            ScoreManager.scoreTest2.Value += 0;
        }
    }
}
