using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BeetleLaser : Projectile
{
    // Laser should only hit the Players that it pierces through once.
    private HashSet<GameObject> damagedObjects = new HashSet<GameObject>();

    // Override the Start method to call the base class's Start first
    new void Start()
    {
        // Call the base class's Start method
        base.Start();
    }

    public override void TriggerEnter2DLogic(Collider2D other)
    {
        if (other.gameObject.tag == "Shield")
        {
            DestroyServerRpc();
        }

        if (other.gameObject.tag == "Player" && !damagedObjects.Contains(other.gameObject))
        {
            // Damage the players
            other.transform.GetComponent<PlayerController>().TakeDamageServerRpc(Damage, other.transform.GetComponent<NetworkObject>().OwnerClientId);
            instantiatingArm.ChargeUltimate(Damage, 100);

            // Add the GameObject to the set of damaged objects to track it
            damagedObjects.Add(other.gameObject);
        }
    }
}
