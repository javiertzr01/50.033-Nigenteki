using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BasicProjectile : Projectile
{

    public override void TriggerEnter2DLogic(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            isColliding = true;
            // Damage Enemy Player
            if (other.transform.GetComponent<PlayerController>().teamId.Value != teamId.Value)
            {
                other.transform.GetComponent<PlayerController>().TakeDamageServerRpc(Damage, other.transform.GetComponent<NetworkObject>().OwnerClientId);
                //instantiatingArm.ChargeUltimate(Damage, 100);

                DestroyServerRpc();
            }
        }
        else if (other.gameObject.tag == "Shield") { }
        else if (other.gameObject.tag == "Projectile") { }
        else
        {
            DestroyServerRpc();
        }
        
    }

}