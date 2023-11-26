using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeetleLaser : Projectile
{
    // Laser should only hit the Players that it pierces through once.
    private HashSet<GameObject> damagedObjects = new HashSet<GameObject>();
    Beetle arm;

    // Override the Start method to call the base class's Start first
    new void Start()
    {
        // Call the base class's Start method
        base.Start();

        // Now, you can execute BeetleLaser specific logic if needed
        arm = instantiatingArm.GetComponent<Beetle>();
    }

    public override void TriggerEnter2DLogic(Collider2D other)
    {
        if (other.gameObject.tag == "Shield")
        {
            DestroyServerRpc();
        }

        if (other.gameObject.tag == "Player" && !damagedObjects.Contains(other.gameObject))
        {
            // Damage the player
            Debug.Log("BeetleLaser: HIT " + other.gameObject.name);
            arm.ChargeUltimate(Damage, 15);

            // Add the GameObject to the set of damaged objects to track it
            damagedObjects.Add(other.gameObject);
        }
    }
}
