using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileVariables", menuName = "ScriptableObjects/Combat/ProjectileVariables", order = 3)]
public class ProjectileVariables : ScriptableObject
{
    public string projectileName;
    public float damage;
    public float maxDistance;

}