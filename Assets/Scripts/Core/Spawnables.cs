using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Spawnables : NetworkBehaviour
{
    [SerializeField]
    public Arm instantiatingArm = null;
    public NetworkVariable<int> teamId = new NetworkVariable<int>();
}
