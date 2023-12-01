using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
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

    public override void TriggerEnter2DLogic(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            // Damage Enemy Player
            if (other.transform.GetComponent<PlayerController>().teamId.Value != teamId.Value && !damagedObjects.Contains(other.GetComponent<NetworkObject>().NetworkObjectId))
            {
                other.transform.GetComponent<PlayerController>().TakeDamageServerRpc(Damage, other.transform.GetComponent<NetworkObject>().OwnerClientId);
                instantiatingArm.ChargeUltimate(Damage, 100);

                // Track the NetworkObjects hit
                damagedObjects.Add(other.GetComponent<NetworkObject>().NetworkObjectId);
            }
        }
        else if (other.gameObject.tag == "Shield")
        {
            ShieldTrigger shield = other.GetComponent<ShieldTrigger>();
            if (shield != null && teamId.Value != shield.teamId.Value)
            {
                shield.TakeDamageServerRpc(Damage, shield.GetComponent<NetworkObject>().OwnerClientId);
                DestroyServerRpc();
            }
        }
        else if (other.gameObject.tag == "Projectile") { }
        else { }
    }
}
