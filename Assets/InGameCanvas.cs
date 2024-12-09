using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameCanvas : MonoBehaviour
{
    [SerializeField]
    Transform scoreBoard;

    [SerializeField]
    GameObject PlayerScoreTemplate;

    [SerializeField]
    public GameObject player;

    void OnEnable()
    {
        NetworkPlayer.OnPlayerSpawn += OnPlayerSpawned;
    }

    void OnDisable()
    {
        NetworkPlayer.OnPlayerSpawn -= OnPlayerSpawned;
    }

    private void OnPlayerSpawned(GameObject @object)
    {
        GameObject PlayerUI = Instantiate(PlayerScoreTemplate, scoreBoard);
        PlayerUI.GetComponent<PlayerScore>().TrackPlayer(player);
        
    }
}
