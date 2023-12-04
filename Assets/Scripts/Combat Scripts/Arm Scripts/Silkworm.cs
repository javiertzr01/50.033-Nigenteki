using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Silkworm : Arm
{
    [SerializeField] private GameObject ultShootPoint;

    // Declare a Queue to track active spellProjectiles
    [SerializeField]
    public List<GameObject> activeSpellProjectiles = new List<GameObject>();

    private void Update()
    {
        // Update the skill charge timer
        if (SkillCharges < maxSkillCharges)
        {
            SkillCoolDown -= Time.deltaTime;
            if (SkillCoolDown <= 0)
            {
                // Increment skillCharges when the timer reaches 0
                SkillCharges++;
                Debug.Log("Increment Silkworm Skill Charge: " + SkillCharges);
                // Reset the skill charge timer
                SkillCoolDown = armVariable.skillCoolDown;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public override void CastBasicAttackServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;

        if (OwnerClientId != clientId) return;

        if (Time.time >= nextBasicFireTime)
        {
            Logger.Instance.LogInfo($"Cast Basic Attack ServerRpc called by {clientId}");

            GameObject basicProjectileClone = SpawnProjectile<Projectile>(clientId, basicProjectile, shootPoint);
            FireProjectile(basicProjectileClone);

            // Set the nextBasicFireTime
            nextBasicFireTime = Time.time + armVariable.baseFireRate;


            // Cast the Basic Attack ClientRpc
            CastBasicAttackClientRpc(new ClientRpcParams        // REMOVE : This just notifies the client
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            });
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public override void CastSkillServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;

        if (OwnerClientId != clientId) return;


        if (SkillCharges > 0)
        {
            // Check if the number of skill instantiations exceeds the maximum
            if (activeSpellProjectiles.Count >= maxSkillInstantiations)
            {
                // Remove the oldest skillProjectile
                GameObject oldestProjectile = activeSpellProjectiles[0];
                oldestProjectile.transform.GetComponent<NetworkObject>().Despawn(true);
                Destroy(oldestProjectile);

                ProjectileListRemoveAt(activeSpellProjectiles, 0);
                Debug.Log("Removed oldest Silkworm skillProjectile");
            }

            // Instantiate the skill projectile and add it to the active projectiles queue
            GameObject skillProjectile = SpawnProjectile<SkillObject>(clientId, spellProjectile, shootPoint);
            ProjectileListAdd(activeSpellProjectiles, skillProjectile);

            // Decrease the number of available skill charges
            SkillCharges--;
            Debug.Log("Decrease Silkworm Skill Charge: " + SkillCharges);
        }

        // Cast the Skill ClientRpc
        CastSkillClientRpc(new ClientRpcParams              // REMOVE :This just notifies the client
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientId }
            }
        });
    }


    [ServerRpc(RequireOwnership = false)]
    public override void CastUltimateServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;

        if (OwnerClientId != clientId) return;


        if (UltimateCharge >= 100f)
        {
            Logger.Instance.LogInfo($"Cast Ultimate ServerRpc called by {clientId}");

            Debug.Log("SILKWORM ULTIMATE: Casting");
            UltimateCharge = 0f; // Reset Ultimate Charge
                                 // Instantiate the ultimate area effect
            GameObject ultimateArea = SpawnProjectile<SkillObject>(clientId, ultimateProjectile, ultShootPoint);
            ultimateArea.GetComponent<SilkRoad>().countdownTimer = armVariable.ultimateDuration;


            // Cast the Ultimate ClientRpc
            CastUltimateClientRpc(new ClientRpcParams       // REMOVE :This just notifies the client
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            });
        }
    }

    public void ProjectileListAdd(List<GameObject> projectileList, GameObject gameObject)   // SERVER ONLY
    {
        projectileList.Add(gameObject);
        ProjectileListAddClientRpc(gameObject.GetComponent<NetworkObject>().NetworkObjectId);
    }

    [ClientRpc]
    public void ProjectileListAddClientRpc(ulong objectId)
    {
        activeSpellProjectiles.Add(NetworkManager.Singleton.SpawnManager.SpawnedObjects[objectId].gameObject);
    }

    public void ProjectileListRemoveAt(List<GameObject> projectileList, int index)  // SERVER ONLY
    {
        projectileList.RemoveAt(index);
        ProjectileListRemoveClientRpc(index);
    }

    [ClientRpc]
    public void ProjectileListRemoveClientRpc(int index)
    {
        activeSpellProjectiles.RemoveAt(index);
    }
}