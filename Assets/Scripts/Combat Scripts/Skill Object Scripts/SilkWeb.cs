using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SilkWeb : SkillObject
{

    public override void TriggerEnter2DLogic(Collider2D other)
    {
        // Check if the colliding object has an enemy PlayerController script
        PlayerController playerController = other.GetComponent<PlayerController>();
        if (playerController != null && playerController.teamId.Value != teamId.Value)
        {
            // Apply the stun effect to the player
            playerController.ApplyStun(3f); // Stun for 3 seconds
            Debug.Log(OwnerClientId);
            RemoveAndShiftServerRpc(NetworkObjectId);
            // Destroy the SilkWeb object
            DestroyServerRpc();
        }
    }

    public override void TriggerExit2DLogic(Collider2D other)
    {
        // throw new System.NotImplementedException();
    }

    private int RemoveAndShift(GameObject itemToRemove)
    {
        Silkworm arm = (Silkworm)instantiatingArm;
        // Find the index of the item to remove
        int indexToRemove = arm.activeSpellProjectiles.IndexOf(itemToRemove);

        // If the item is found, remove it and shift the remaining elements
        if (indexToRemove != -1)
        {
            // Debug.Log("Index to Remove: " + indexToRemove);
            arm.activeSpellProjectiles.RemoveAt(indexToRemove);
        }
        return indexToRemove;
    }

    [ServerRpc(RequireOwnership = false)]
    public void RemoveAndShiftServerRpc(ulong id)
    {
        GameObject itemToRemove = NetworkManager.Singleton.SpawnManager.SpawnedObjects[id].gameObject;
        int indexToRemove = RemoveAndShift(itemToRemove);
        RemoveAndShiftClientRpc(indexToRemove);
    }

    [ClientRpc]
    public void RemoveAndShiftClientRpc(int index)
    {
        if (IsHost) return;
        Silkworm arm = (Silkworm) instantiatingArm;
        arm.activeSpellProjectiles.RemoveAt(index);
    }

}
