using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class BeetleLaser : Projectile
{
    // Laser should only hit the Players that it pierces through once.
    private HashSet<ulong> damagedObjects = new HashSet<ulong>();

    // Override the Start method to call the base class's Start first
    new void Start()
    {
        // Call the base class's Start method
        base.Start();
    }

    public override void OnEnemyPlayerTriggerEnter2D(Collider2D other)
    {
        if (!damagedObjects.Contains(other.GetComponent<NetworkObject>().NetworkObjectId))
        {
            base.OnEnemyPlayerTriggerEnter2D(other);
            damagedObjects.Add(other.GetComponent<NetworkObject>().NetworkObjectId);
        }
    }

    public override void OnEnemyShieldTriggerEnter2D(Collider2D other)
    {
        ShieldTrigger shield = other.GetComponent<ShieldTrigger>();     // TODO: Change this
        if (shield != null && teamId.Value != shield.teamId.Value)
        {
            shield.TakeDamageServerRpc(Damage, shield.GetComponent<NetworkObject>().OwnerClientId);
            DestroyServerRpc();
        }
    }
}