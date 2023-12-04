using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SpawnPointBehaviour : MonoBehaviour
{
    [SerializeField]
    private int teamId; // Set this in the inspector

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!NetworkManager.Singleton.IsServer) // Ensure this runs only on the server
        {
            return;
        }

        PlayerController player = collision.gameObject.GetComponent<PlayerController>();

        if (player != null && player.teamId.Value == this.teamId)
        {
            player.HealPlayerServerRpc(50 * Time.deltaTime, player.GetComponent<NetworkObject>().OwnerClientId);
        }
    }
}
