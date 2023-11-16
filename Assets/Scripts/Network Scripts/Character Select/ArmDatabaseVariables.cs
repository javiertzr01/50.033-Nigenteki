using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ArmDatabaseVariables", menuName = "NetworkScriptableObjects/ArmDatabaseVariables", order = 10)]
public class ArmDatabaseVariables : ScriptableObject
{
    [SerializeField] private BuildArmVariables[] arms = new BuildArmVariables[0];

    public BuildArmVariables[] GetAllArms() => arms;

    public BuildArmVariables GetArmById(int id)
    {
        foreach (var arm in arms)
        {
            if (arm.Id == id)
            {
                return arm;
            }
        }

        return null;
    }
}
