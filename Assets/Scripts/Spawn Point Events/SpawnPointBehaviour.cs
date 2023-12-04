using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SpawnPointBehaviour : MonoBehaviour
{
    [SerializeField]
    private int teamId; // Set this in the inspector
    [SerializeField]
    private float spawnHealingValue = 50f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        SpawnProtection(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        SpawnProtection(other);
        HealAllyPlayer(other);
    }

    private void HealAllyPlayer(Collider2D other)
    {
        if (!NetworkManager.Singleton.IsServer) // Ensure this runs only on the server
        {
            return;
        }

        PlayerController player = other.gameObject.GetComponent<PlayerController>();

        if (player != null && player.teamId.Value == this.teamId)
        {
            player.HealPlayerServerRpc(spawnHealingValue * Time.deltaTime, player.GetComponent<NetworkObject>().OwnerClientId);
        }
    }

    private void SpawnProtection(Collider2D other)
    {
        Projectile projectile = other.gameObject.GetComponent<Projectile>();
        SkillObject skillObject = other.gameObject.GetComponent<SkillObject>();

        if (projectile != null && projectile.teamId.Value != this.teamId)
        {
            Logger.Instance.LogInfo("Destroy Projectile in Spawn");
            projectile.DestroyServerRpc();
        }
        else if (skillObject != null && skillObject.teamId.Value != this.teamId)
        {
            skillObject.DestroyServerRpc();
        }
    }
}
