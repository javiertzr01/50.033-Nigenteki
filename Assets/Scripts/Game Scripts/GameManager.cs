using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    public UnityEvent GameEndEvent;

    [HideInInspector]
    private static float phaseOneDuration = 390.5f; //30.5f; //780.5f // 13mins


    [HideInInspector]
    private static float phaseTwoDuration = 210.5f; //25.5f; //420.5f // 7mins

    [HideInInspector]
    public static float winCondition = 0f; // Points needed for a team to win
    [HideInInspector]
    public static float teamInitialTimer = 90.5f; //20.5f; // 180.5f; 
    [HideInInspector]
    public static float phaseOneMaxCaptureDuration = 30.5f; // 10.5f; // 60.5f; 
    [HideInInspector]
    public static float phaseOneMaxCaptureTimer = teamInitialTimer - phaseOneMaxCaptureDuration; // 60.5f; 

    public GameStateStore gameStateStore;

    [SerializeField]
    private bool _gameInProgress = true;

    private int team1Kills = 0;
    private int team1Deaths = 0;
    private int team2Kills = 0;
    private int team2Deaths = 0;

    private void Start()
    {
        StartGame();
    }

    private void Update()
    {
        if (GameInProgress)
        {
            // Assuming you call a method to update the timer
            UpdateMainTimer();
            ControlPointGameLogic();

        }
    }

    private void ControlPointGameLogic()
    {
        if (gameStateStore.isControlPointActive.Value)
        {
            if (gameStateStore.currentTeamOnControlPoint.Value == 1)
            {
                if ((gameStateStore.phase.Value == 1 && gameStateStore.team1Timer.Value >= phaseOneMaxCaptureTimer) || gameStateStore.phase.Value == 2)
                {
                    ReduceTeamTimer(1, CaptureTimeMultiplier(Time.deltaTime, gameStateStore.numberOfTeam1PlayersOnControlPoint.Value));
                }
            }
            else if (gameStateStore.currentTeamOnControlPoint.Value == 2)
            {
                if ((gameStateStore.phase.Value == 1 && gameStateStore.team2Timer.Value >= phaseOneMaxCaptureTimer) || gameStateStore.phase.Value == 2)
                {
                    ReduceTeamTimer(2, CaptureTimeMultiplier(Time.deltaTime, gameStateStore.numberOfTeam2PlayersOnControlPoint.Value));
                }
            }
        }
    }

    private float CaptureTimeMultiplier(float deltaTime, int numberOfPlayers)
    {
        if (numberOfPlayers == 1)
        {
            return deltaTime;
        }
        else if (numberOfPlayers == 2)
        {
            return deltaTime * (4 / 3); // for 7.5s of deltaTime, would return 10s
        }
        else if (numberOfPlayers == 3)
        {
            return deltaTime * 2f; // for 5s of deltaTime, would return 10s
        }
        else
        {
            return 0;
        }
    }

    public void StartGame()
    {
        GameInProgress = true;
        SetTimeScaleClientRpc(1); // Call the RPC method
        StartPhaseOne();
    }

    public void EndGame()
    {
        GameInProgress = false;
        SetTimeScaleClientRpc(0); // Call the RPC method
        gameStateStore.phase.Value = 0;
        GameEndEvent.Invoke();
        // Clean up the game, disable components, etc.
    }

    [ClientRpc]
    private void SetTimeScaleClientRpc(float timeScale)
    {
        Time.timeScale = timeScale;
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

    private void UpdateMainTimer()
    {
        

        if (gameStateStore.mainTimer.Value <= 0f)
        {
            if (gameStateStore.phase.Value == 1)
            {
                gameStateStore.mainTimer.Value -= Time.deltaTime;

                StartPhaseTwo();
            }
            else if (gameStateStore.phase.Value == 2)
            {
                CheckForKDRWinner();
            }
        }
        else
        {
            gameStateStore.mainTimer.Value -= Time.deltaTime;
        }
    }

    private void StartPhaseOne()
    {
        gameStateStore.mainTimer.Value = phaseOneDuration;
        gameStateStore.team1Timer.Value = teamInitialTimer;
        gameStateStore.team2Timer.Value = teamInitialTimer;
        gameStateStore.phase.Value = 1;
        gameStateStore.gameWinner.Value = -1;
    }

    private void StartPhaseTwo()
    {
        // Transition to phase two
        gameStateStore.mainTimer.Value = phaseTwoDuration;

        // Raise the phase change event
        gameStateStore.phase.Value = 2;
    }


    public void ReduceTeamTimer(int team, float clockedTime)
    {
        if (gameStateStore.team1Timer.Value > winCondition && gameStateStore.team2Timer.Value > winCondition)
        {
            if (team == 1)
            {
                gameStateStore.team1Timer.Value = AdjustTimerValue(gameStateStore.team1Timer.Value, clockedTime);
            }
            else if (team == 2)
            {
                gameStateStore.team2Timer.Value = AdjustTimerValue(gameStateStore.team2Timer.Value, clockedTime);
            }

            // Check for win condition
            if (gameStateStore.team1Timer.Value <= winCondition || gameStateStore.team2Timer.Value <= winCondition)
            {
                DeclareWinner(gameStateStore.team1Timer.Value < gameStateStore.team2Timer.Value ? 1 : 2);
            }
        }

        
    }

    private float AdjustTimerValue(float currentTeamTimer, float clockedTime)
    {
        float timer = currentTeamTimer;
        timer -= clockedTime;

        if (gameStateStore.phase.Value == 1 && timer < phaseOneMaxCaptureTimer)
        {
            timer = phaseOneMaxCaptureTimer;
        }
        else if (gameStateStore.phase.Value == 1 && timer < winCondition)
        {
            timer = winCondition;
        }


        return timer;
    }

    private void DeclareWinner(int winningTeam)
    {
        // Raise the game win event with winning team info
        gameStateStore.gameWinner.Value = winningTeam; // 0 - Tie; 1 - Team 1; 2 - Team 2;

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
    public float GetmainTimer()
    {
        return gameStateStore.mainTimer.Value;
    }
}

