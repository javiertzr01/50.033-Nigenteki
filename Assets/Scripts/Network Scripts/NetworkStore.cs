using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;

public class NetworkStore : NetworkBehaviour
{
    public NetworkVariable<GeneratedMapData> generatedMapData = new NetworkVariable<GeneratedMapData>(
        new GeneratedMapData {
            FloorPositionsArray = new Vector2Int[0],
            SpritesArray = new Sprites[0],
            ObstaclePositionsArray = new Vector2Int[0],
            ObstacleNamesArray = new string[0],
            RedSpawnPosition = new Vector2Int(),
            BlueSpawnPosition = new Vector2Int(),
            CapturePointPosition = new Vector2Int()
        }
        );

    public override void OnNetworkSpawn()
    { 
        generatedMapData.OnValueChanged += OnGeneratedMapDataChanged;
    }

    public void OnGeneratedMapDataChanged(GeneratedMapData previous, GeneratedMapData current)
    {
        Logger.Instance.LogInfo($"Generated map data has been updated");
    }

}
