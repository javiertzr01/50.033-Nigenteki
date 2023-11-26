using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Unity.Netcode;
using TMPro;
using System;

public class UIManager : NetworkBehaviour
{
    public NetworkStore netStore;

    public UnityEvent gameStart;


    [SerializeField]
    private TextMeshProUGUI playersInGameText;

    [SerializeField]
    private TMP_InputField joinCodeInputField;

    [SerializeField]
    private TextMeshProUGUI mainTimerText;
    
    [SerializeField]
    private TextMeshProUGUI phaseObjectiveText;
    
    [SerializeField]
    private TextMeshProUGUI team1TimerText;
    
    [SerializeField]
    private TextMeshProUGUI team2TimerText;

    [SerializeField]
    private TextMeshProUGUI gameWinLossText;

    private float team1Percentage;
    private float team2Percentage;

    private void Awake()
    {
        Cursor.visible = true;
    }

    // Start is called before the first frame update
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {

    }

    public void UpdateMainTimerText(float timeInSeconds)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(timeInSeconds);
        mainTimerText.text = $"{timeSpan.ToString(@"mm\:ss")}";
    }



    public void UpdatePhaseObjectiveText(int phase)
    {
        if (phase == 1)
        {
            phaseObjectiveText.text = $"Resource Collection Phase: Collect Resource & Find Control Point!";
        }
        else if (phase == 2)
        {
            phaseObjectiveText.text = $"Control Point Phase: Capture Control Point!";
        }
        else if (phase == 0)
        {
            phaseObjectiveText.text = $"Game Over!";
        }
        else
        {
            phaseObjectiveText.text = $"";
        }
        
    }

    public void UpdateGameWinLossText(int winner)
    {
        if (winner == 1)
        {
            gameWinLossText.text = $"Team 1 Has Won!";
        }
        else if (winner == 2)
        {
            gameWinLossText.text = $"Team 2 Has Won!";
        }
        else if (winner == 0)
        {
            gameWinLossText.text = $"Tied Match!";
        }
        else
        {
            gameWinLossText.text = $"";
        }

    }

    public void Team1TimerText(float secondsLeft)
    {
        //team1TimerText.text = $"Team1: {percent.ToString("00.00")}%";
        TimeSpan timeSpan = TimeSpan.FromSeconds(secondsLeft);
        team1TimerText.text = $"Team 1: {timeSpan.ToString(@"m\:ss")}";
        Team1Percentage((secondsLeft / GameManager.teamInitialTimer)*100);
    }

    public void Team1Percentage(float percent)
    {
        //team1TimerText.text = $"Team1: {percent.ToString("00.00")}%";
        team1Percentage = percent;
        // todo: update UI
    }

    public void Team2Percentage(float percent)
    {
        //team2TimerText.text = $"Team2: {percent.ToString("00.00")}%";
        team2Percentage = percent;
        // todo: update UI
    }

    public void Team2TimerText(float secondsLeft)
    {
        //team2TimerText.text = $"Team 2: {percent.ToString("00.00")}";
        TimeSpan timeSpan = TimeSpan.FromSeconds(secondsLeft);
        team2TimerText.text = $"Team 2: {timeSpan.ToString(@"m\:ss")}";
        Team2Percentage((secondsLeft / GameManager.teamInitialTimer) * 100);
    }


}
