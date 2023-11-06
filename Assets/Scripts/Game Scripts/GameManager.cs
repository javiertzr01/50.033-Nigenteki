using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public UnityEvent<float> TimerUpdateEvent;
    public UnityEvent<int> PhaseUpdateEvent;
    public UnityEvent<float> Team1TimerUpdateEvent;
    public UnityEvent<float> Team1PercentUpdateEvent;
    public UnityEvent<float> Team2TimerUpdateEvent;
    public UnityEvent<float> Team2PercentUpdateEvent;
    public UnityEvent<int> GameWinLossEvent;
    public UnityEvent GameEndEvent;

    private float phaseOneDuration = 10.5f; //780.5f // 13mins
    private float phaseTwoDuration = 5.5f; //420.5f // 7mins
    private static float winCondition = 0f; // Points needed for a team to win
    private static float teamInitialTimer = 12f; // 180f; 

    [SerializeField]
    private bool _gameInProgress = false;

    private float currentTimer;
    private int currentPhase = 1;
    private int team1Kills = 0;
    private int team1Deaths = 0;
    private int team2Kills = 0;
    private int team2Deaths = 0;

    private float team1PercentLeft;
    private float team2PercentLeft;

    private float _team1Timer;
    private float _team2Timer;
    public float Team1Timer
    {
        get { return _team1Timer; }
        set { _team1Timer = Mathf.Max(value, 0); } // Ensures that _team1Timer never goes below 0
    }

    public float Team2Timer
    {
        get { return _team2Timer; }
        set { _team2Timer = Mathf.Max(value, 0); } // Ensures that _team2Timer never goes below 0
    }


    private void Start()
    {
    }


    private void Update()
    {
        if (GameInProgress)
        {
            // Assuming you call a method to update the timer
            UpdateTimer();
        }
        
    }

    public void StartGame()
    {
        GameInProgress = true;
        Time.timeScale = 1;
        StartPhaseOne();
    }

    public void EndGame()
    {
        GameInProgress = false;
        Time.timeScale = 0;
        currentPhase = 0;
        PhaseUpdateEvent.Invoke(currentPhase);

        GameEndEvent.Invoke();

        // Clean up the game, disable components, etc.
    }

    public bool GameInProgress
    {
        get { return _gameInProgress; }
        set
        {
            if (_gameInProgress != value)
            {
                _gameInProgress = value;
            }
        }
    }

    private void UpdateTimer()
    {
        currentTimer -= Time.deltaTime;
        ReduceTeamTimer(1, Time.deltaTime);

        if (currentTimer <= 0f)
        {
            if (currentPhase == 1)
            {
                TimerUpdateEvent.Invoke(currentTimer);
                StartPhaseTwo();
            }
            else if (currentPhase == 2)
            {
                CheckForKDRWinner();
            }
        }
        else
        {
            TimerUpdateEvent.Invoke(currentTimer);
        }
    }

    private void StartPhaseOne()
    {
        currentTimer = phaseOneDuration;
        Team1Timer = teamInitialTimer;
        Team2Timer = teamInitialTimer;
        team1PercentLeft = 100f;
        team2PercentLeft = 100f;

        Team1TimerUpdateEvent.Invoke(Team1Timer);
        Team1PercentUpdateEvent.Invoke(team1PercentLeft);

        Team2TimerUpdateEvent.Invoke(Team2Timer);
        Team2PercentUpdateEvent.Invoke(team2PercentLeft);

        currentPhase = 1;
        PhaseUpdateEvent.Invoke(currentPhase);
        GameWinLossEvent.Invoke(-1);
    }

    private void StartPhaseTwo()
    {
        // Transition to phase two
        currentPhase = 2;
        currentTimer = phaseTwoDuration;

        // Raise the phase change event
        PhaseUpdateEvent.Invoke(currentPhase);
    }


    public void ReduceTeamTimer(int team, float clockedTime)
    {
        if (Team1Timer > winCondition && Team2Timer > winCondition)
        {
            if (team == 1)
            {
                Team1Timer -= clockedTime;
                Team1TimerUpdateEvent.Invoke(Team1Timer);
                Team1PercentUpdateEvent.Invoke((Team1Timer / teamInitialTimer) * 100f);
            }
            else if (team == 2)
            {
                Team2Timer -= clockedTime;
                Team2TimerUpdateEvent.Invoke(Team2Timer);
                Team2PercentUpdateEvent.Invoke((Team2Timer / teamInitialTimer) * 100f);
            }

            // Check for win condition
            if (Team1Timer <= winCondition || Team2Timer <= winCondition)
            {
                DeclareWinner(Team1Timer < Team2Timer ? 1 : 2);
            }
        }

        
    }

    private void DeclareWinner(int winningTeam)
    {
        // Raise the game win event with winning team info
        GameWinLossEvent.Invoke(winningTeam); // 0 - Tie; 1 - Team 1; 2 - Team 2;

        // End the game
        EndGame();
    }


    private void CheckForKDRWinner()
    {
        float team1KDR = team1Deaths > 0 ? team1Kills / (float)team1Deaths : team1Kills; // avoid division by zero.
        float team2KDR = team2Deaths > 0 ? team2Kills / (float)team2Deaths : team2Kills; // avoid division by zero.

        // Compare scores and declare winner or tie
        if (team1KDR > team2KDR)
        {
            DeclareWinner(1);
        }
        else if (team1KDR < team2KDR)
        {
            DeclareWinner(2);
        }
        else
        {
            DeclareWinner(0);
        }
    }


    // Call this method to get the remaining time for UI display or other logic
    public float GetCurrentTimer()
    {
        return currentTimer;
    }
}

