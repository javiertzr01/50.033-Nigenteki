using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkStore : NetworkBehaviour
{
    [Header("Lobby Variables")]
    public NetworkVariable<int> playersInLobby = new NetworkVariable<int>();

    [Header("Map Generator Variables")]
    public NetworkVariable<HashSet<Vector2Int>> generatedMapFloorPositions = new NetworkVariable<HashSet<Vector2Int>>();
}
