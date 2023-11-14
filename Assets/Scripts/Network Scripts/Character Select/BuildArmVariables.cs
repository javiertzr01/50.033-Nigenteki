using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[CreateAssetMenu(fileName = "BuildArmVariables", menuName = "NetworkScriptableObjects/BuildArmVariables", order = 8)]
public class BuildArmVariables : ScriptableObject
{
    [SerializeField] private int id = -1;
    [SerializeField] private string displayName = "New Arm Display Name";
    [SerializeField] private Sprite icon;
    [SerializeField] private GameObject armPrefab;

    public int Id => id;
    public string DisplayName => displayName;
    public Sprite Icon => icon;
    public GameObject ArmPrefab => armPrefab;
}
