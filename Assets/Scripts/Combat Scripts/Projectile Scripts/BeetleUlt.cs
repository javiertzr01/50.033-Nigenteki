using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BeetleUlt : Projectile
{
    // Laser should only hit the Plaeyrs that it pierces through once.
    private HashSet<GameObject> damagedObjects = new HashSet<GameObject>();

    public override void TriggerEnter2DLogic(Collider2D other)
    {
        if (other.gameObject.tag == "Projectile")
        {
            other.gameObject.GetComponent<Projectile>().DestroyServerRpc();
        }
        if (other.gameObject.tag == "Shield")
        {
            DestroyServerRpc();
        }

        if (other.gameObject.tag == "Player" && !damagedObjects.Contains(other.gameObject))
        {
            // Damage the player
            Debug.Log("BeetleUlt: HIT " + other.gameObject.name);
            // collision.gameObject.GetComponent<PlayerController>().currentHealth -= _damage;

            // Add the GameObject to the set of damaged objects to track it
            damagedObjects.Add(other.gameObject);
        }
    }

}
