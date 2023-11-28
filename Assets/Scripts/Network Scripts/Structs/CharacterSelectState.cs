using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Netcode;
using UnityEngine;

public struct CharacterSelectState : INetworkSerializable, IEquatable<CharacterSelectState>
{
    public ulong ClientId;
    public int CharacterId;
    public int TeamId; // 0: red, 1: blue

    public CharacterSelectState(ulong clientId, int characterId = -1, int teamId = -1, bool isLockedIn = false)
    {
        ClientId = clientId;
        CharacterId = characterId;
        TeamId = teamId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref CharacterId);
        serializer.SerializeValue(ref TeamId);
    }

    public bool Equals(CharacterSelectState other)
    {
        return ClientId == other.ClientId &&
            CharacterId == other.CharacterId &&
            TeamId == other.TeamId;
    }
}
