using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ControlPointController : NetworkBehaviour
{

    public GameStateStore gameStateStore;
    public NetworkStore netStore;
    private ControlPointTeamOccupancyState currentOccupancyState;
    enum ControlPointTeamOccupancyState
    {
        NoPlayers,
        BothTeamsPresent,
        OnlyTeam1Present,
        OnlyTeam2Present
    }

    // Start is called before the first frame update
    void Start()
    {
        ControlPointPositionServerRpc();
        gameStateStore.isControlPointActive.Value = false;
        gameStateStore.currentTeamOnControlPoint.Value = 0;
        currentOccupancyState = ControlPointTeamOccupancyState.NoPlayers;
    }

    // Update is called once per frame
    void Update()
    {
        // todo: If isControlPointActive==true, Check if players are still alive/not-destroyed
        EvaluateControlPointOccupancy();
        UpdateControlPointStatus();

    }

    [ServerRpc(RequireOwnership = false)]
    public void ControlPointPositionServerRpc()
    {
        transform.position = new Vector3(netStore.generatedMapData.Value.CapturePointPosition[0], netStore.generatedMapData.Value.CapturePointPosition[1], 0);
    }

    private void UpdateControlPointStatus()
    {
        switch (currentOccupancyState)
        {
            case ControlPointTeamOccupancyState.OnlyTeam1Present:
                gameStateStore.isControlPointActive.Value = true;
                gameStateStore.currentTeamOnControlPoint.Value = 1;
                // Call Team 1
                break;

            case ControlPointTeamOccupancyState.OnlyTeam2Present:
                gameStateStore.isControlPointActive.Value = true;
                gameStateStore.currentTeamOnControlPoint.Value = 2;
                break;

            case ControlPointTeamOccupancyState.NoPlayers:
            case ControlPointTeamOccupancyState.BothTeamsPresent:
            default:
                gameStateStore.isControlPointActive.Value = false;
                gameStateStore.currentTeamOnControlPoint.Value = 0;
                break;
        }
    }

    private void EvaluateControlPointOccupancy()
    {
        if (gameStateStore.numberOfTeam1PlayersOnControlPoint.Value <= 0 && gameStateStore.numberOfTeam2PlayersOnControlPoint.Value <= 0)
        {
            currentOccupancyState = ControlPointTeamOccupancyState.NoPlayers;
        }
        else if (gameStateStore.numberOfTeam1PlayersOnControlPoint.Value > 0 && gameStateStore.numberOfTeam2PlayersOnControlPoint.Value > 0)
        {
            currentOccupancyState = ControlPointTeamOccupancyState.BothTeamsPresent;
        }
        else if (gameStateStore.numberOfTeam1PlayersOnControlPoint.Value > 0)
        {
            currentOccupancyState = ControlPointTeamOccupancyState.OnlyTeam1Present;
        }
        else // gameStateStore.numberOfTeam2PlayersOnControlPoint.Value > 0
        {
            currentOccupancyState = ControlPointTeamOccupancyState.OnlyTeam2Present;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //todo: Check Team of Player
            if (collision.transform.GetComponent<PlayerController>().teamId.Value == 0)
            {
                gameStateStore.numberOfTeam1PlayersOnControlPoint.Value += 1;
            }
            else if (collision.transform.GetComponent<PlayerController>().teamId.Value == 1)
            {
                gameStateStore.numberOfTeam2PlayersOnControlPoint.Value += 1;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //todo: Check Team of Player
            if (collision.transform.GetComponent<PlayerController>().teamId.Value == 0)
            {
                gameStateStore.numberOfTeam1PlayersOnControlPoint.Value -= 1;
            }
            else if (collision.transform.GetComponent<PlayerController>().teamId.Value == 1)
            {
                gameStateStore.numberOfTeam2PlayersOnControlPoint.Value -= 1;
            }

        }
    }
}
