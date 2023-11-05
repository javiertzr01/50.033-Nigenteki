using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ArmVariables", menuName = "ScriptableObjects/Combat/ArmVariables", order = 3)]
public class ArmVariables : ScriptableObject
{

    public string armName;
    public float baseForce;
    public float baseFireRate;
    public float skillForce;
    public float skillCoolDown;
    public float skillDuration;
    public float ultimateForce;
    public int ultimateCharge;
    public float ultimateDuration;

    [Space(10f), Header("Shield")]
    public float shieldMaxHealth;

}
