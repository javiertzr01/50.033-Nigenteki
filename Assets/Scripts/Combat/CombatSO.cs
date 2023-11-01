using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[CreateAssetMenu(fileName = "CombatSO", menuName = "Combat/CombatSO", order = 0)]
public class CombatSO : ScriptableObject
{
    [Header("BasicShooter")]
    [Range(1.0f, 100.0f)]
    public float basicShooterSpeed;

    [Space(10), Header("BasicShield")]
    [Range(100, 1000)]
    public int basicShieldHealth;


    [Space(10), Header("Silkworm")]
    [Range(1.0f, 100.0f)]
    public float silkwormBulletSpeed;
}