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
            // Damage Enemy Player
            if (other.transform.GetComponent<PlayerController>().teamId.Value != teamId.Value)
            {
                other.transform.GetComponent<PlayerController>().TakeDamageServerRpc(Damage, other.transform.GetComponent<NetworkObject>().OwnerClientId);
                //instantiatingArm.ChargeUltimate(Damage, 100);

                InstantiateDamageNumberServerRpc(other.transform.GetComponent<NetworkObject>().OwnerClientId);

                DestroyServerRpc();
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
        else if (other.gameObject.tag == "ControlPoint") { }
        else
        {
            DestroyServerRpc();
        }

    }

}