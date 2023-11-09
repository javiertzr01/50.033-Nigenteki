using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildPlayerVariables", menuName = "NetworkScriptableObjects/BuildPlayerVariables", order = 7)]
public class BuildPlayerVariables : ScriptableObject
{
    [SerializeField] private int id = -1;
    [SerializeField] private int leftArmId = 0;
    [SerializeField] private int rightArmId = 0;
    [SerializeField] private string displayName = "New Display Name";
    [SerializeField] private Sprite icon;
    [SerializeField] private Sprite leftArmIcon;
    [SerializeField] private Sprite rightArmIcon;

    public int Id => id;
    public int LeftArmId => leftArmId;
    public int RightArmId => RightArmId;
    public string DisplayName => displayName;
    public Sprite Icon => icon;
    public Sprite LeftArmIcon => leftArmIcon;
    public Sprite RightArmIcon => rightArmIcon;
}
