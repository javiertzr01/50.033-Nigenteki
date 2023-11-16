using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public struct GeneratedMapData : INetworkSerializable
{
    public Vector2Int[] FloorPositionsArray;
    public Sprites[] SpritesArray;
    public Vector2Int[] ObstaclePositionsArray;
    public string[] ObstacleNamesArray;
    public Vector2Int RedSpawnPosition;
    public Vector2Int BlueSpawnPosition;
    public Vector2Int CapturePointPosition;


    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref FloorPositionsArray);
        serializer.SerializeValue(ref SpritesArray);
        serializer.SerializeValue(ref ObstaclePositionsArray);

        //serializer.SerializeValue(ref ObstacleNamesArray);
        var length = 0;
        if (!serializer.IsReader)
            length = ObstacleNamesArray.Length;

        serializer.SerializeValue(ref length);
        if (serializer.IsReader)
            ObstacleNamesArray = new string[length];

        for (var n = 0; n < length; ++n)
        {
            serializer.SerializeValue(ref ObstacleNamesArray[n]);
        }

        serializer.SerializeValue(ref RedSpawnPosition);
        serializer.SerializeValue(ref BlueSpawnPosition);
        serializer.SerializeValue(ref CapturePointPosition);

    }
}
