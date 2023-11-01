using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ArmVariables", menuName = "ScriptableObjects/ArmVariables", order = 2)]
public class ArmVariables : ScriptableObject
{

    public string armName;
    
    public float baseDamage;
    public float baseForce;
    public float baseFireRate;

    public float skillDamage;
    public float skillForce;
    public float skillFireRate;

    public float ultimateDamage;
    public float ultimateForce;
    public float ultimateFireRate;

}
