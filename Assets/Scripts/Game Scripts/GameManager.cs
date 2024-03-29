using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public UnityEvent GameEndEvent;

    [HideInInspector]
    private static float phaseOneDuration = 240f; //4mins

    [HideInInspector]
    private static float phaseTwoDuration = 360f; //6mins

    [HideInInspector]
    public static float winCondition = 0f; // Points needed for a team to win
    [HideInInspector]
    public static float teamInitialTimer = 90f; //1.5 mins
    [HideInInspector]
    public static float phaseOneMaxCaptureDuration = 30f; // 30 seconds 
    [HideInInspector]
    public static float phaseOneMaxCaptureTimer = teamInitialTimer - phaseOneMaxCaptureDuration; // 1 min; 

    public GameStateStore gameStateStore;

    [SerializeField]
    private bool _gameInProgress = true;

    private int team1Kills = 0;
    private int team1Deaths = 0;
    private int team2Kills = 0;
    private int team2Deaths = 0;

    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private void Start()
    {
        StartGame();
    }

    private void Update()
    {
        if (GameInProgress)
        {
            // Assuming you call a method to update the timer
            UpdateMainTimerServerRpc();
            ControlPointGameLogicServerRpc();

        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ControlPointGameLogicServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        if (OwnerClientId != clientId) return;
        ControlPointGameLogic();
    }

    private void ControlPointGameLogic()
    {
        if (gameStateStore.isControlPointActive.Value)
        {
            if (gameStateStore.currentTeamOnControlPoint.Value == 1)
            {
                if ((gameStateStore.phase.Value == 1 && gameStateStore.team1Timer.Value >= phaseOneMaxCaptureTimer) || gameStateStore.phase.Value == 2)
                {
                    float captureMultiplier = CaptureTimeMultiplier(Time.deltaTime, gameStateStore.numberOfTeam1PlayersOnControlPoint.Value);
                    ReduceTeamTimer(1, captureMultiplier);
                }
            }
            else if (gameStateStore.currentTeamOnControlPoint.Value == 2)
            {
                if ((gameStateStore.phase.Value == 1 && gameStateStore.team2Timer.Value >= phaseOneMaxCaptureTimer) || gameStateStore.phase.Value == 2)
                {
                    float captureMultiplier = CaptureTimeMultiplier(Time.deltaTime, gameStateStore.numberOfTeam2PlayersOnControlPoint.Value);
                    ReduceTeamTimer(2, captureMultiplier);
                }
            }
            else
            {
                gameStateStore.isControlPointActiveReducingTimer.Value = false;
            }
        }
        else
        {
            gameStateStore.isControlPointActiveReducingTimer.Value = false;
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
            return deltaTime * 1.5f; //(4.0f / 3.0f); // for 7.5s of deltaTime, would return 10s
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
        gameStateStore.isControlPointActiveReducingTimer.Value = false;

        GameInProgress = false;
        SetTimeScaleClientRpc(0); // Call the RPC method
        gameStateStore.phase.Value = 0;
        GameEndEvent.Invoke();
        // Clean up the game, disable components, etc.
    }

    public void MainMenu()
    {
        if (IsServer)
        {
            ForceMainMenuClientRpc();
        }
        ReturnToMainMenuCleanup();
    }

    private void ReturnToMainMenuCleanup()
    {
        Time.timeScale = 1;
        NetworkManager.Singleton.Shutdown();
        if(IsServer)
        {
            Destroy(ServerManager.Instance.gameObject);
        }
        Destroy(AudioManager.Instance.gameObject);
        Destroy(NetworkManager.gameObject);
        SceneManager.LoadScene(mainMenuSceneName, LoadSceneMode.Single);
    }

    [ClientRpc]
    private void SetTimeScaleClientRpc(float timeScale)
    {
        Time.timeScale = timeScale;
    }

    [ClientRpc]
    private void ForceMainMenuClientRpc()
    {
        if (!IsHost)
            ReturnToMainMenuCleanup();
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

    [ServerRpc(RequireOwnership = false)]
    public void UpdateMainTimerServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        if (OwnerClientId != clientId) return;
        UpdateMainTimer();
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
        StartPhaseOneServerRpc();
    }


    [ServerRpc(RequireOwnership = false)]
    public void StartPhaseOneServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        if (OwnerClientId != clientId) return;

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
            else
            {
                gameStateStore.isControlPointActiveReducingTimer.Value = false;
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
            gameStateStore.isControlPointActiveReducingTimer.Value = false;
        }
        else if (gameStateStore.phase.Value == 1 && timer < winCondition)
        {
            timer = winCondition;
            gameStateStore.isControlPointActiveReducingTimer.Value = false;
        }
        else
        {
            gameStateStore.isControlPointActiveReducingTimer.Value = true;
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


    private void ProcessTeamKD()
    {
        team1Kills = 0;
        team1Deaths = 0;
        team2Kills = 0;
        team2Deaths = 0;
        foreach (var playerClientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            PlayerController player = NetworkManager.Singleton.ConnectedClients[playerClientId].PlayerObject.transform.GetComponent<PlayerController>();
            if(player.teamId.Value == 1)
            {
                team1Kills += player.kills.Value;
                team1Deaths += player.deaths.Value;
            }
            else if(player.teamId.Value == 2)
            {
                team2Kills += player.kills.Value;
                team2Deaths += player.deaths.Value;
            }

        }


    }

    private void CheckForKDRWinner()
    {
        ProcessTeamKD();
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

