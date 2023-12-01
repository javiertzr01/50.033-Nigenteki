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
        EvaluateControlPointOccupancy();
        UpdateControlPointStatusServerRpc();

    }

    [ServerRpc(RequireOwnership = false)]
    public void ControlPointPositionServerRpc(ServerRpcParams serverRpcParams = default)
    {
        transform.position = new Vector3(netStore.generatedMapData.Value.CapturePointPosition[0], netStore.generatedMapData.Value.CapturePointPosition[1], 0);
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateControlPointStatusServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        if (OwnerClientId != clientId) return;

        UpdateControlPointStatus();
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
            OnTriggerEnter2DServerRpc(collision.transform.GetComponent<PlayerController>().teamId.Value, collision.transform.GetComponent<PlayerController>().OwnerClientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnTriggerEnter2DServerRpc(int teamId, ulong clientId, ServerRpcParams serverRpcParams = default)
    {

        if (teamId == 0)
        {
            //CHECKS FOR DUPLICATE
            if (!gameStateStore.team1PlayersIDOnControlPoint.Contains(clientId))
            {
                gameStateStore.team1PlayersIDOnControlPoint.Add(clientId);
            }
            gameStateStore.numberOfTeam1PlayersOnControlPoint.Value = gameStateStore.team1PlayersIDOnControlPoint.Count;
        }
        else if (teamId == 1)
        {
            if (!gameStateStore.team2PlayersIDOnControlPoint.Contains(clientId))
            {
                gameStateStore.team2PlayersIDOnControlPoint.Add(clientId);
            }
            gameStateStore.numberOfTeam2PlayersOnControlPoint.Value = gameStateStore.team2PlayersIDOnControlPoint.Count;
        }

    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnTriggerExit2DServerRpc(collision.transform.GetComponent<PlayerController>().teamId.Value, collision.transform.GetComponent<PlayerController>().OwnerClientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnTriggerExit2DServerRpc(int teamId, ulong clientId, ServerRpcParams serverRpcParams = default)
    {
        if (teamId == 0)
        {
            if (gameStateStore.team1PlayersIDOnControlPoint.Contains(clientId))
            {
                gameStateStore.team1PlayersIDOnControlPoint.Remove(clientId);
            }
            gameStateStore.numberOfTeam1PlayersOnControlPoint.Value = gameStateStore.team1PlayersIDOnControlPoint.Count;
        }
        else if (teamId == 1)
        {
            if (gameStateStore.team2PlayersIDOnControlPoint.Contains(clientId))
            {
                gameStateStore.team2PlayersIDOnControlPoint.Remove(clientId);
            }
            gameStateStore.numberOfTeam2PlayersOnControlPoint.Value = gameStateStore.team2PlayersIDOnControlPoint.Count;
        }

    }
}
