using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SilkWeb : SkillObject
{
    public override void CollisionEnter2DLogic(Collision2D collider)
    {
        // throw new System.NotImplementedException();
    }

    public override void TriggerEnter2DLogic(Collider2D other)
    {
        // Check if the colliding object has a PlayerController script
        PlayerController playerController = other.GetComponent<PlayerController>();

        if (playerController != null)
        {
            // Apply the stun effect to the player
            playerController.ApplyStun(5f); // Stun for 5 seconds
        }

        // Destroy the SilkWeb object
        Destroy(gameObject);
    }

    public override void TriggerExit2DLogic(Collider2D other)
    {
        // throw new System.NotImplementedException();
    }
}
