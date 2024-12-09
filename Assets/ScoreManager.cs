using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;    

public class ScoreManager : NetworkBehaviour
{
    public Text scorePlayer1;
    public Text scorePlayer2;

    public static float scoreCount;

    public static NetworkVariable<float> scoreTest1 = new NetworkVariable<float>(0);
    public static NetworkVariable<int> scoreTest2 = new NetworkVariable<int>(0);


    private void Update()
    {
        scorePlayer1.text = "Player 1 Score: " + Mathf.Round(scoreTest1.Value);
        scorePlayer2.text = "Player 2 Score: " + Mathf.Round(scoreTest2.Value);
    }
}
