using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocustDagger : Projectile
{

    public override void TriggerEnter2DLogic(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            // Damage the player
            // collision.gameObject.GetComponent<PlayerController>().currentHealth -= _damage;
            instantiatingArm.ChargeUltimate(Damage, 30);
            DestroyServerRpc();
        }
        else if (other.gameObject.tag == "Shield")
        {
            instantiatingArm.ChargeUltimate(Damage, 30);
            DestroyServerRpc();
        }
    }

}
