using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


public class TimerUI : NetworkBehaviour
{
    [SerializeField] Text timerText;
    [SerializeField] Text GameOver;
   
    public static NetworkVariable<float> remainTime = new NetworkVariable<float>(30);

    public static bool isTimer = false;


    private void Update()
    {
       

        

        int minutes = Mathf.FloorToInt( remainTime.Value / 60 );
        int seconds = Mathf.FloorToInt( remainTime.Value % 60 );

        timerText.text = remainTime.Value.ToString();//string.Format("{0:00}:{1:00}", minutes, seconds);

        if(remainTime.Value == 0)
        {
            if(ScoreManager.scoreTest1.Value > ScoreManager.scoreTest2.Value )
            {
                GameOver.text = "Player 1 Wins!";
            }
            else if(ScoreManager.scoreTest2.Value > ScoreManager.scoreTest1.Value )
            {
                GameOver.text = "Player 2 Wins";
            }
        }
    }

    public void onStart()
    {
        isTimer = true;
    }
    public void onStop() 
    { 
        isTimer = false; 
    }
}
