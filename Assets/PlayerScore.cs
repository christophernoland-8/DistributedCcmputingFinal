using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Collections;
using System;

public class PlayerScore : MonoBehaviour
{
    [SerializeField]
    public TMP_Text NameUI;

    [SerializeField]
    public TMP_Text ScoreUI;


    public void TrackPlayer(GameObject player)
    {
        player.GetComponent<PlayerData>().Name.OnValueChanged += OnNameChanged;
        player.GetComponent<PlayerData>().Score.OnValueChanged += OnScoreChanged;
        OnScoreChanged(0, player.GetComponent<PlayerData>().Score.Value);
        OnNameChanged("", player.GetComponent<PlayerData>().Name.Value);
    }

    private void OnScoreChanged(int previousValue, int newValue)
    {
        ScoreUI.text = newValue.ToString();
    }

    private void OnNameChanged(FixedString128Bytes previousValue, FixedString128Bytes newValue)
    {
        NameUI.text = newValue.ToString();
    }
}
