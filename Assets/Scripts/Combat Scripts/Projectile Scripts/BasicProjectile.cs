using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : Projectile
{
    public override void CollisionEnter2DLogic(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // Damage the player
            // collision.gameObject.GetComponent<PlayerController>().currentHealth -= _damage;
        }
        Destroy(gameObject);
    }

    public override void TriggerEnter2DLogic(Collider2D other)
    {
        if (other.gameObject.tag == "Shield")
        {
            Destroy(gameObject);
        }
    }

    public override void Initialize()
    {
        base.Initialize();
    }
}
