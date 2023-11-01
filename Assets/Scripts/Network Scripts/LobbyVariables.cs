using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[CreateAssetMenu(fileName = "LobbyVariables", menuName = "NetworkScriptableObjects/LobbyVariables", order = 0)]
public class LobbyVariables : ScriptableObject
{
    public NetworkVariable<int> playersInLobby = new NetworkVariable<int>();
}
