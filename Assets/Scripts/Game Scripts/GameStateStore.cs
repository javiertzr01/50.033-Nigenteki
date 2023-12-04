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
    public NetworkList<ulong> team1PlayersIDOnControlPoint;
    public NetworkVariable<int> numberOfTeam1PlayersOnControlPoint = new NetworkVariable<int>();
    public NetworkList<ulong> team2PlayersIDOnControlPoint;
    public NetworkVariable<int> numberOfTeam2PlayersOnControlPoint = new NetworkVariable<int>();
    public NetworkVariable<int> currentTeamOnControlPoint = new NetworkVariable<int>();


    public UnityEvent<float> mainTimerUpdateEventInvoker;
    public UnityEvent<float> team1TimerUpdateEventInvoker;
    public UnityEvent<float> team2TimerUpdateEventInvoker;
    public UnityEvent<int> phaseUpdateEventInvoker;
    public UnityEvent<int> gameWinnerUpdateEventInvoker;

    public UnityEvent<bool> activeStateOfCPEventInvoker;
    public UnityEvent<int> numberOfTeam1PlayersOnCPEventInvoker;
    public UnityEvent<int> numberOfTeam2PlayersOnCPEventInvoker;
    public UnityEvent<int> currentTeamOnCPEventInvoker;

    public void Awake()
    {
        isControlPointActive.Value = false;
        team1PlayersIDOnControlPoint = new NetworkList<ulong>();
        team2PlayersIDOnControlPoint = new NetworkList<ulong>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        mainTimer.OnValueChanged += OnMainTimerChanged;
        team1Timer.OnValueChanged += OnTeam1TimerChanged;
        team2Timer.OnValueChanged += OnTeam2TimerChanged;
        phase.OnValueChanged += OnPhaseChanged;
        gameWinner.OnValueChanged += OnGameWinnerChanged;

        isControlPointActive.OnValueChanged += OnControlPointActiveStateChanged;
        numberOfTeam1PlayersOnControlPoint.OnValueChanged += OnNumberOfTeam1PlayersOnControlPointChanged;
        numberOfTeam2PlayersOnControlPoint.OnValueChanged += OnNumberOfTeam2PlayersOnControlPointChanged;
        currentTeamOnControlPoint.OnValueChanged += OnCurrentTeamOnControlPointChanged;
    }

    public void OnCurrentTeamOnControlPointChanged(int previous, int current)
    {
        //Logger.Instance.LogInfo($"OnCurrentTeamOnControlPointChanged: {current.ToString()}");
        currentTeamOnCPEventInvoker.Invoke(current);
    }

    public void OnNumberOfTeam1PlayersOnControlPointChanged(int previous, int current)
    {
        //Logger.Instance.LogInfo($"OnNumberOfTeam1PlayersOnControlPointChanged: {current.ToString()}");
        numberOfTeam1PlayersOnCPEventInvoker.Invoke(current);
    }

    public void OnNumberOfTeam2PlayersOnControlPointChanged(int previous, int current)
    {
        //Logger.Instance.LogInfo($"OnNumberOfTeam2PlayersOnControlPointChanged: {current.ToString()}");
        numberOfTeam2PlayersOnCPEventInvoker.Invoke(current);
    }

    public void OnControlPointActiveStateChanged(bool previous, bool current)
    {
        //Logger.Instance.LogInfo($"ControlPointActiveState: {current.ToString()}");
        activeStateOfCPEventInvoker.Invoke(current);
    }

    public void OnGameWinnerChanged(int previous, int current)
    {
        Logger.Instance.LogInfo($"Current Game Winner: {current.ToString()}");
        if (current != -1)
            gameWinnerUpdateEventInvoker.Invoke(current);
    }

    public void OnPhaseChanged(int previous, int current)
    {
        //Logger.Instance.LogInfo($"Current Phase: {current.ToString()}");
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
