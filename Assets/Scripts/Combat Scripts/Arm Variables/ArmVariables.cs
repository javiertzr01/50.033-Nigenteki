using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ArmVariables", menuName = "ScriptableObjects/Combat/ArmVariables", order = 3)]
public class ArmVariables : ScriptableObject
{

    public string armName;
    [Space(10f), Header("Basic Attack")]
    public float baseForce;
    public float baseFireRate;
    [Space(10f), Header("Skill")]
    public float skillForce;
    public float skillCoolDown;
    public float skillDuration;
    public int skillMaxCharges;
    public int skillMaxInstants;
    [Space(10f), Header("Ultimate")]
    public float ultimateForce;
    public float ultimateFireRate;
    public int ultimateCharge;
    public float ultimateDuration;

    [Space(10f), Header("Shield-related Variables")]
    public float shieldMaxHealth;

}
