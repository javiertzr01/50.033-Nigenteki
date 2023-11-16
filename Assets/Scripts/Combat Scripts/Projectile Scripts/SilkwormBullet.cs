using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SilkwormBullet : Projectile
{

    public override void CollisionEnter2DLogic(Collision2D collision)
    {
        DestroyServerRpc();
    }

    public override void TriggerEnter2DLogic(Collider2D other)
    {
        // if (other.gameObject.tag == "Player")
        // {
        //     // Damage the player
        //     // collision.gameObject.GetComponent<PlayerController>().currentHealth -= _damage;
        //     arm.ChargeUltimate(Damage, 15);
        // }

        if (other.gameObject.tag == "Shield")
        {
            instantiatingArm.ChargeUltimate(Damage, 30);
            DestroyServerRpc();
        }
    }

}
