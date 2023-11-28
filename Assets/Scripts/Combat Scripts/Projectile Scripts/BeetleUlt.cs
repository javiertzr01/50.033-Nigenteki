using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class BeetleUlt : Projectile
{
    // Laser should only hit the Players that it pierces through once.
    private HashSet<GameObject> damagedObjects = new HashSet<GameObject>();

    public override void TriggerEnter2DLogic(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && !damagedObjects.Contains(other.gameObject))
        {
            // Damage Enemy Player
            if (other.transform.GetComponent<PlayerController>().teamId.Value != teamId.Value)
            {
                other.transform.GetComponent<PlayerController>().TakeDamageServerRpc(Damage, other.transform.GetComponent<NetworkObject>().OwnerClientId);
                instantiatingArm.ChargeUltimate(Damage, 100);

                // Add the GameObject to the set of damaged objects to track it
                damagedObjects.Add(other.gameObject);
            }
        }
        else if (other.gameObject.tag == "Shield")
        {
            // Ignore Ally Shields
        }
        else if (other.gameObject.tag == "Projectile")
        {
            // Destroy Enemy Projectiles
            if (other.transform.GetComponent<Projectile>().teamId.Value != teamId.Value)
            {
                other.transform.GetComponent<Projectile>().DestroyServerRpc();
            }
        }
        else { }
    }

}
