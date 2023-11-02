using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ArmVariables", menuName = "ScriptableObjects/ArmVariables", order = 2)]
public class ArmVariables : ScriptableObject
{

    public string armName;
    public float baseForce;
    public float baseFireRate;
    public float skillForce;
    public float skillCoolDown;
    public float ultimateForce;
    public int ultimateCharge;

}
