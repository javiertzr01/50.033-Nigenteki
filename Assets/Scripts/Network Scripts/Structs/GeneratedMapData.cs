using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public struct GeneratedMapData : INetworkSerializable
{
    public int Size;
    public Vector2Int[] FloorPositions;
    public Biomes[] BiomeTypes;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Size);
        serializer.SerializeValue(ref FloorPositions);
        serializer.SerializeValue(ref BiomeTypes);
    }
}
