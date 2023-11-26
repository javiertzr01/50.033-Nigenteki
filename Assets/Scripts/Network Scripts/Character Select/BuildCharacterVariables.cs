using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[CreateAssetMenu(fileName = "BuildCharacterVariables", menuName = "NetworkScriptableObjects/BuildCharacterVariables", order = 7)]
public class BuildCharacterVariables : ScriptableObject
{
    [SerializeField] private int id = -1;
    [SerializeField] private int teamId = -1;
    [SerializeField] private string displayName = "New Display Name";
    [SerializeField] private Sprite icon;
    [SerializeField] private GameObject characterPrefab;

    public int Id => id;
    public int TeamId => teamId;
    public string DisplayName => displayName;
    public Sprite Icon => icon;
    public GameObject CharacterPrefab => characterPrefab;
}
