using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ArmVariables", menuName = "ScriptableObjects/ProjectileVariables", order = 2)]
public class ProjectileVariables : ScriptableObject
{

    public string projectileName;
    public float damage;
    public float maxDistance;

}