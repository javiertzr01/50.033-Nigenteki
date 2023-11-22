using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Locust : Arm
{
    private float nextBasicFireTime = 0f;
    private int skillCharges;
    private int maxSkillCharges;
    private float skillChargeTimer;
    GameObject shotBasicProjectileClone;
    private bool ulted;

    public override void Initialize()
    {
        base.Initialize();
        maxSkillCharges = armVariable.skillMaxCharges;
        skillChargeTimer = armVariable.skillCoolDown;

        skillCharges = maxSkillCharges; // Initialize Arm with the maximum skill Charges first

        UltimateCharge = armVariable.ultimateCharge;
        ulted = false;

    }

    private void Update()
    {
        // Update the skill charge timer
        if (skillCharges < maxSkillCharges)
        {
            skillChargeTimer -= Time.deltaTime;
            if (skillChargeTimer <= 0)
            {
                // Increment skillCharges when the timer reaches 0
                skillCharges++;
                Debug.Log("Increment Locust Skill Charge: " + skillCharges);
                // Reset the skill charge timer
                skillChargeTimer = armVariable.skillCoolDown;
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
            // Check if the projectile instantiations exceeds 1
            if (shotBasicProjectileClone == null)
            {
                Logger.Instance.LogInfo($"Cast Basic Attack ServerRpc called by {clientId}");

                // Instantiate the Projectile
                shotBasicProjectileClone = Instantiate(basicProjectile, shootPoint.transform.position, transform.rotation);
                shotBasicProjectileClone.transform.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
                // Set the instantiatingArm
                shotBasicProjectileClone.GetComponent<Projectile>().instantiatingArm = gameObject.GetComponent<Arm>();
                // Set max distance and apply force
                // shotBasicProjectileClone.GetComponent<Projectile>().MaxDistance = 1f;
                Rigidbody2D rb = shotBasicProjectileClone.GetComponent<Rigidbody2D>();
                rb.AddForce(shootPoint.transform.up * armVariable.baseForce, ForceMode2D.Impulse);
                // Cast the Basic Attack ClientRpc
                CastBasicAttackClientRpc(new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[] { clientId }
                    }
                });
                // Set the nextBasicFireTime
                nextBasicFireTime = Time.time + armVariable.baseFireRate;
            }
        }
    }


    [ClientRpc]
    public override void CastBasicAttackClientRpc(ClientRpcParams clientRpcParams = default)
    {
        if (!IsOwner) return;
        Logger.Instance.LogInfo($"Cast Basic Attack ClientRpc called by {OwnerClientId}");
    }


    [ServerRpc(RequireOwnership = false)]
    public override void CastSkillServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;

        if (OwnerClientId != clientId) return;


        if (skillCharges > 0)
        {
            // TODO: Apply a force to the player in the direction of the mouse such that they "dash"
            // Get the grandparent GameObject
            PlayerController playerController = transform.parent?.parent.GetComponent<PlayerController>();
            if (playerController != null)
            {
                // Calculate the dash direction based on the forward direction
                Vector2 dashDirection = playerController.transform.up;

                if (playerController != null)
                {
                    // Apply the dash force or movement to the grandparent GameObject
                    playerController.Dash(dashDirection);
                }

                // Decrease the number of available skill charges
                skillCharges--;
                Debug.Log("Decrease Locust Skill Charge: " + skillCharges);
            }

            // Cast the Skill ClientRpc
            CastSkillClientRpc(new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            });
        }
    }


    [ClientRpc]
    public override void CastSkillClientRpc(ClientRpcParams clientRpcParams = default)
    {
        if (!IsOwner) return;
        Logger.Instance.LogInfo($"Cast Skill ClientRpc called by {OwnerClientId}");
    }



    [ServerRpc(RequireOwnership = false)]
    public override void CastUltimateServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;

        if (OwnerClientId != clientId) return;


        if (UltimateCharge >= 100f)
        {
            Logger.Instance.LogInfo($"Cast Ultimate ServerRpc called by {clientId}");

            Debug.Log("LOCUST ULTIMATE: Casting");
            UltimateCharge = 0f; // Reset Ultimate Charge
                                 // Instantiate the ultimate area effect

            ulted = true;
            // Cast the Ultimate ClientRpc
            CastUltimateClientRpc(new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            });
        }
    }


    [ClientRpc]
    public override void CastUltimateClientRpc(ClientRpcParams clientRpcParams = default)
    {
        if (!IsOwner) return;
        Logger.Instance.LogInfo($"Cast Ultimate ClientRpc called by {OwnerClientId}");
    }

}