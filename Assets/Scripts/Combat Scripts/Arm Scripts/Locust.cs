using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Locust : Arm
{
    private GameObject ultimateProjectile;
    private float nextBasicFireTime = 0f;
    private int skillCharges;
    private int maxSkillCharges;
    private float skillChargeTimer;
    GameObject shotBasicProjectileClone;
    private bool ulted;
    private float countdownTimer;
    private PlayerController playerController;

    public override void Initialize()
    {
        base.Initialize();
        playerController = transform.parent?.parent.GetComponent<PlayerController>();

        maxSkillCharges = armVariable.skillMaxCharges;
        skillChargeTimer = armVariable.skillCoolDown;

        skillCharges = maxSkillCharges; // Initialize Arm with the maximum skill Charges first

        UltimateCharge = armVariable.ultimateCharge;
        countdownTimer = armVariable.ultimateDuration;
        ulted = false;

        if (projectiles[1] != null)
        {
            ultimateProjectile = projectiles[1];
        }

    }

    public override bool HaveSkillCharges()
    {
        return true;
    }

    public override int GetSkillCharges()
    {
        return skillCharges;
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
        // Update the ultimate timer
        if (ulted)
        {
            countdownTimer -= Time.deltaTime;
            if (countdownTimer <= 0f)
            {
                ulted = false; // Reset the ulted flag when the timer reaches 0
                playerController.immuneStun.Value = false;
                Debug.Log("LOCUST ULT: Expired");
                countdownTimer = armVariable.ultimateDuration; // Reset the timer for the next ultimate
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
            // Limit number of projectiles to 1 UNLESS ULTED
            if (ulted || shotBasicProjectileClone == null)
            {
                Logger.Instance.LogInfo($"Cast Basic Attack ServerRpc called by {clientId}");

                // Instantiate the type of Projectile
                if (ulted)
                {
                    shotBasicProjectileClone = Instantiate(ultimateProjectile, shootPoint.transform.position, transform.rotation);

                }
                else
                {
                    shotBasicProjectileClone = Instantiate(basicProjectile, shootPoint.transform.position, transform.rotation);
                }
                shotBasicProjectileClone.layer = transform.root.gameObject.layer;
                // Setup teamId
                shotBasicProjectileClone.GetComponent<Projectile>().teamId.Value = transform.root.transform.GetComponent<PlayerController>().teamId.Value;
                shotBasicProjectileClone.transform.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
                // Set the instantiatingArm
                shotBasicProjectileClone.GetComponent<Projectile>().instantiatingArm = gameObject.GetComponent<Arm>();
                // Set max distance and apply force
                // shotBasicProjectileClone.GetComponent<Projectile>().MaxDistance = 1f;
                Rigidbody2D rb = shotBasicProjectileClone.GetComponent<Rigidbody2D>();
                rb.AddForce(shootPoint.transform.up * armVariable.baseForce, ForceMode2D.Impulse);

                //Audio Player
                int attackTypeIndex = 0; //Basic - 0; Skill - 1; Ultimate - 2;
                CastAttackSFXServerRpc(attackTypeIndex, serverRpcParams);

                // Cast the Basic Attack ClientRpc
                CastBasicAttackClientRpc(new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[] { clientId }
                    }
                });


                // Set the nextBasicFireTime
                if (ulted)
                {
                    nextBasicFireTime = Time.time + armVariable.ultimateFireRate;
                }
                else
                {
                    nextBasicFireTime = Time.time + armVariable.baseFireRate;
                }

            }
        }
    }


    [ClientRpc]
    public override void CastBasicAttackClientRpc(ClientRpcParams clientRpcParams = default)
    {
        if (!IsOwner) return;
        Logger.Instance.LogInfo($"Cast Basic Attack ClientRpc called by {OwnerClientId}");
    }


    // [ServerRpc(RequireOwnership = false)]
    // public override void CastSkillServerRpc(ServerRpcParams serverRpcParams = default)
    // {
    //     var clientId = serverRpcParams.Receive.SenderClientId;

    //     if (OwnerClientId != clientId) return;

    //     // Limit number of skill charges to 2 UNLESS ULTED
    //     if (ulted || skillCharges > 0)
    //     {
    //         if (playerController != null)
    //         {
    //             // Calculate the dash direction based on the forward direction
    //             Vector2 dashDirection = playerController.transform.up;

    //             if (playerController != null)
    //             {
    //                 // Apply the dash force or movement to the grandparent GameObject
    //                 playerController.Dash(dashDirection);
    //             }

    //             // Decrease the number of available skill charges
    //             if (!ulted)
    //             {
    //                 skillCharges--;
    //                 Debug.Log("Decrease Locust Skill Charge: " + skillCharges);
    //             }
    //         }

    //         // Cast the Skill ClientRpc
    //         CastSkillClientRpc(new ClientRpcParams
    //         {
    //             Send = new ClientRpcSendParams
    //             {
    //                 TargetClientIds = new ulong[] { clientId }
    //             }
    //         });
    //     }
    // }

    [ServerRpc(RequireOwnership = false)]
    public override void CastSkillServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;

        if (OwnerClientId != clientId) return;

        if (ulted || skillCharges > 0)
        {
            //Audio Player
            int attackTypeIndex = 1; //Basic - 0; Skill - 1; Ultimate - 2;
            CastAttackSFXServerRpc(attackTypeIndex, serverRpcParams);


            // Trigger the dash on the client
            playerController.TriggerDashClientRpc(new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            });

            // Decrease the number of available skill charges
            if (!ulted)
            {
                skillCharges--;
                Debug.Log("Decrease Locust Skill Charge: " + skillCharges);
            }
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
            ulted = true;
            playerController.immuneStun.Value = true;

            //Audio Player
            int attackTypeIndex = 2; //Basic - 0; Skill - 1; Ultimate - 2;
            CastAttackSFXServerRpc(attackTypeIndex, serverRpcParams);

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