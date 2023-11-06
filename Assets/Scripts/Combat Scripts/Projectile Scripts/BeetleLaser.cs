using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeetleLaser : Projectile
{
    // Laser should only hit the Plaeyrs that it pierces through once.
    private HashSet<GameObject> damagedObjects = new HashSet<GameObject>();
    Beetle arm;

    void Start()
    {
        arm = instantiatingArm.GetComponent<Beetle>();
    }

    public override void CollisionEnter2DLogic(Collision2D collision)
    {
        // No Collision Logic as it should "pierce" objects and players
    }

    public override void TriggerEnter2DLogic(Collider2D other)
    {
        if (other.gameObject.tag == "Shield")
        {

            Destroy(gameObject);
        }

        if (other.gameObject.tag == "Player" && !damagedObjects.Contains(other.gameObject))
        {
            // Damage the player
            Debug.Log("BeetleLaser: HIT " + other.gameObject.name);
            arm.ChargeUltimate(Damage, 15);
            // collision.gameObject.GetComponent<PlayerController>().currentHealth -= _damage;

            // Add the GameObject to the set of damaged objects to track it
            damagedObjects.Add(other.gameObject);
        }
    }

    public override void Initialize()
    {
        base.Initialize();
    }
}
