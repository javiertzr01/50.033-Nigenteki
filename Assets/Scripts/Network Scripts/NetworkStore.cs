using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;

public class NetworkStore : NetworkBehaviour
{

    public UnityEvent updatePlayersInLobbyUI;
    public UnityEvent loadMap;

    public NetworkVariable<int> playersInLobby = new NetworkVariable<int>();

    public NetworkVariable<GeneratedMapData> generatedMapData = new NetworkVariable<GeneratedMapData>(
        new GeneratedMapData {
            Size = 0,
            FloorPositions = new Vector2Int[0]
        }
        );

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        playersInLobby.OnValueChanged += OnPlayersInLobbyChanged;
        generatedMapData.OnValueChanged += OnGeneratedMapDataChanged;
    }

    public void OnPlayersInLobbyChanged(int previous, int current)
    {
        Logger.Instance.LogInfo($"Players in lobby has changed to {current.ToString()}");

        updatePlayersInLobbyUI.Invoke();
    }

    public void OnGeneratedMapDataChanged(GeneratedMapData previous, GeneratedMapData current)
    {
        Logger.Instance.LogInfo($"Generated map data has been updated");
    }

}
