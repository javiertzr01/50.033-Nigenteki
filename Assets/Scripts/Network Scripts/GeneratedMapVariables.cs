using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[CreateAssetMenu(fileName = "GeneratedMapVariables", menuName = "NetworkScriptableObjects/GeneratedMapVariables", order = 1)]
public class GeneratedMapVariables : ScriptableObject
{
    public NetworkVariable<HashSet<Vector2Int>> savedFloorPositions = new NetworkVariable<HashSet<Vector2Int>>();
}
