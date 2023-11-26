using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LocustDagger : Projectile
{

    public override void TriggerEnter2DLogic(Collider2D other)
    {
        // TODO: Team Separation

        if (other.gameObject.tag == "Player")
        {
            other.transform.GetComponent<PlayerController>().TakeDamageServerRpc(Damage, other.transform.GetComponent<NetworkObject>().OwnerClientId);
            instantiatingArm.ChargeUltimate(Damage, 100);
            DestroyServerRpc();

        }
        // else if (other.gameObject.tag == "Shield")
        // {
        //     instantiatingArm.ChargeUltimate(Damage, 30);
        //     DestroyServerRpc();
        // }
    }

}
