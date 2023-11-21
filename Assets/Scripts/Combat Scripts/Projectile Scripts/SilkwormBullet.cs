using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SilkwormBullet : Projectile
{
    Silkworm arm;
    void Start()
    {
        arm = instantiatingArm.GetComponent<Silkworm>();
    }
    /*public override void CollisionEnter2DLogic(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // Damage the player
            // collision.gameObject.GetComponent<PlayerController>().currentHealth -= _damage;
            arm.ChargeUltimate(Damage, 15);
        }
        Destroy(gameObject);
    }*/

    public override void TriggerEnter2DLogic(Collider2D other)
    {
        if (other.gameObject.tag == "Shield")
        {
            arm.ChargeUltimate(Damage, 30);
            DestroyServerRpc();
        }
    }

}
