using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class BeetleLaser : BasicProjectile
{
    // Laser should only hit the Players that it pierces through once.
    private HashSet<ulong> damagedObjects = new HashSet<ulong>();

    public override void OnEnemyPlayerTriggerEnter2D(Collider2D other)
    {
        if (!damagedObjects.Contains(other.GetComponent<NetworkObject>().NetworkObjectId))
        {
            base.OnEnemyPlayerTriggerEnter2D(other);
            damagedObjects.Add(other.GetComponent<NetworkObject>().NetworkObjectId);
        }
    }
}