using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[CreateAssetMenu(fileName = "GeneratedMapVariables", menuName = "NetworkScriptableObjects/GeneratedMapVariables", order = 1)]
public class GeneratedMapVariables : ScriptableObject
{
    public NetworkList<Vector2Int> savedFloorPositions = new NetworkList<Vector2Int>();
}
