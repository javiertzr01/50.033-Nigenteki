using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;

public class GameStateStore : NetworkBehaviour
{
    public NetworkVariable<float> mainTimer = new NetworkVariable<float>();
    public NetworkVariable<float> team1Timer = new NetworkVariable<float>();
    public NetworkVariable<float> team2Timer = new NetworkVariable<float>();
    public NetworkVariable<int> phase = new NetworkVariable<int>();
    public NetworkVariable<int> gameWinner = new NetworkVariable<int>();

    public NetworkVariable<bool> isControlPointActive = new NetworkVariable<bool>();
    public NetworkVariable<int> numberOfTeam1PlayersOnControlPoint = new NetworkVariable<int>();
    public NetworkVariable<int> numberOfTeam2PlayersOnControlPoint = new NetworkVariable<int>();
    public NetworkVariable<int> currentTeamOnControlPoint = new NetworkVariable<int>();


    public UnityEvent<float> mainTimerUpdateEventInvoker;
    public UnityEvent<float> team1TimerUpdateEventInvoker;
    public UnityEvent<float> team2TimerUpdateEventInvoker;
    public UnityEvent<int> phaseUpdateEventInvoker;
    public UnityEvent<int> gameWinnerUpdateEventInvoker;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        mainTimer.OnValueChanged += OnMainTimerChanged;
        team1Timer.OnValueChanged += OnTeam1TimerChanged;
        team2Timer.OnValueChanged += OnTeam2TimerChanged;
        phase.OnValueChanged += OnPhaseChanged;
        gameWinner.OnValueChanged += OnGameWinnerChanged;

        //isControlPointActive.OnValueChanged += OnTeam1TimerChanged;
        //numberOfTeam1PlayersOnControlPoint.OnValueChanged += OnTeam2TimerChanged;
        //numberOfTeam2PlayersOnControlPoint.OnValueChanged += OnPhaseChanged;
        //currentTeamOnControlPoint.OnValueChanged += OnGameWinnerChanged;


    }

    //public void OnGameWinnerChanged(int previous, int current)
    //{
    //    Logger.Instance.LogInfo($"Current Game Winner: {current.ToString()}");
    //    gameWinnerUpdateEventInvoker.Invoke(current);
    //}

    public void OnGameWinnerChanged(int previous, int current)
    {
        Logger.Instance.LogInfo($"Current Game Winner: {current.ToString()}");
        gameWinnerUpdateEventInvoker.Invoke(current);
    }

    public void OnPhaseChanged(int previous, int current)
    {
        Logger.Instance.LogInfo($"Current Phase: {current.ToString()}");
        phaseUpdateEventInvoker.Invoke(current);
    }

    public void OnMainTimerChanged(float previous, float current)
    {
        //Logger.Instance.LogInfo($"Main Timer CountDown: {current.ToString()}");
        mainTimerUpdateEventInvoker.Invoke(current);
    }

    public void OnTeam1TimerChanged(float previous, float current)
    {
        //Logger.Instance.LogInfo($"Main Timer CountDown: {current.ToString()}");
        team1TimerUpdateEventInvoker.Invoke(current);
    }
    public void OnTeam2TimerChanged(float previous, float current)
    {
        //Logger.Instance.LogInfo($"Main Timer CountDown: {current.ToString()}");
        team2TimerUpdateEventInvoker.Invoke(current);
    }


}
