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
            playerController.ApplyStun(3f); // Stun for 3 seconds
        }

        Silkworm arm = (Silkworm)instantiatingArm;
        RemoveAndShift(arm.activeSpellProjectiles, gameObject);


        // Destroy the SilkWeb object
        DestroyServerRpc();
    }

    public override void TriggerExit2DLogic(Collider2D other)
    {
        // throw new System.NotImplementedException();
    }

    private void RemoveAndShift(List<GameObject> list, GameObject itemToRemove)
    {
        // Find the index of the item to remove
        int indexToRemove = list.IndexOf(itemToRemove);

        // If the item is found, remove it and shift the remaining elements
        if (indexToRemove != -1)
        {
            Debug.Log("Index to Remove: " + indexToRemove);
            list.RemoveAt(indexToRemove);
        }
    }

}
