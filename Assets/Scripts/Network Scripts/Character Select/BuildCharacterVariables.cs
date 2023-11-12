using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildCharacterVariables", menuName = "NetworkScriptableObjects/BuildCharacterVariables", order = 7)]
public class BuildCharacterVariables : ScriptableObject
{
    [SerializeField] private int id = -1;
    [SerializeField] private string displayName = "New Display Name";
    [SerializeField] private Sprite icon;

    public int Id => id;
    public string DisplayName => displayName;
    public Sprite Icon => icon;

}
