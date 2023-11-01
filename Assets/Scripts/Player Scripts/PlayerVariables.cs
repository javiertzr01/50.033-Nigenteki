using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[CreateAssetMenu(fileName = "PlayerVariables", menuName = "ScriptableObjects/PlayerVariables", order = 1)]
public class PlayerVariables : ScriptableObject
{
    public float maxHealth;
    public float moveSpeed;

    public GameObject rightArm;
    public GameObject leftArm;

    public NetworkVariable<float> currentHealth;

}
