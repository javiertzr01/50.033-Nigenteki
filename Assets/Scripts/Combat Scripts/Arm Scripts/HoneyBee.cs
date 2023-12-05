using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class HoneyBee : Arm
{
    private bool ulted = false;
    private GameObject[] players;
    private bool skillReady = true;
    
    public void Update()
    {
        if (!skillReady)
        {
            if (SkillCoolDown > 0.0f)
            {
                SkillCoolDown -= Time.deltaTime;
            }
            else
            {
                Debug.Log("Honeybee Skill can be casted");
                skillReady = true;
            }
        }

        if (ulted)
        {
            if (countdownTimer > 0f)
            {
                countdownTimer -= Time.deltaTime;
            }
            else
            {
                Debug.Log("Honeybee Ultimate ended");
                // Reset
                // Iterate through each player
                foreach (var player in players)
                {
                    // Access the PlayerController script
                    PlayerController playerController = player.GetComponent<PlayerController>();
                    // Check if the playerController is not null and is an ally
                    if (playerController != null && playerController.teamId.Value == transform.root.transform.GetComponent<PlayerController>().teamId.Value)
                    {
                        // Reset the MoveSpeed variable
                        playerController.AdjustMovementSpeedServerRpc(playerController.defaultMoveSpeed);
                        // Reset damage taken
                        playerController.AdjustDamageTakenScaleServerRpc(playerController.defaultDamageTakenScale);
                        playerController.passiveHealthRegenerationPercentage = playerController.defaultPassiveHealthRegenerationPercentage;
                    }
                }
                ulted = false;
            }
        }
    }

    public override void SetProjectiles()
    {
        basicProjectile = projectiles[0];
        spellProjectile = projectiles[1];
    }

    [ServerRpc(RequireOwnership = false)]
    public override void CastBasicAttackServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;

        if (OwnerClientId != clientId) return;

        if (Time.time >= nextBasicFireTime)
        {
            Logger.Instance.LogInfo($"Cast Basic Attack ServerRpc called by {clientId}");

            // Spawn the Projectile
            GameObject projectileClone = SpawnProjectile<Projectile>(clientId, basicProjectile, shootPoint);
            // Fire the projectile
            FireProjectile(projectileClone, armVariable.baseForce);

            //Audio Player
            int attackTypeIndex = 0; //Basic - 0; Skill - 1; Ultimate - 2;
            CastAttackSFXServerRpc(attackTypeIndex, serverRpcParams);
            
            // Cast the Basic Attack ClientRpc
            CastBasicAttackClientRpc(new ClientRpcParams            // REMOVE : This just notifies the client
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

    [ServerRpc(RequireOwnership = false)]
    public override void CastSkillServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;

        if (OwnerClientId != clientId) return;

        if (skillReady)
        {
            Debug.Log("HONEYBEE SKILL: Casting");
            // Instantiate the skill projectile and add it to the active projectiles queue
            GameObject skillProjectile = SpawnProjectile<SkillObject>(clientId, spellProjectile, shootPoint);
            skillProjectile.GetComponent<HoneyComb>().countdownTimer = armVariable.skillDuration;

            // Set the skill cooldown to initial value
            SkillCoolDown = armVariable.skillCoolDown;
            skillReady = false;

            //Audio Player
            int attackTypeIndex = 1; //Basic - 0; Skill - 1; Ultimate - 2;
            CastAttackSFXServerRpc(attackTypeIndex, serverRpcParams);

            // Cast the Skill ClientRpc
            CastSkillClientRpc(new ClientRpcParams                  // REMOVE : This just notifies the client
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            });
        }
        else
        {
            Debug.Log("HONEYBEE SKILL: Cannot cast yet");
            Logger.Instance.LogInfo($"Cast Skill ClientRpc called by {OwnerClientId}: FAIL - CD");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public override void CastUltimateServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;

        if (OwnerClientId != clientId) return;


        if (UltimateCharge >= 100f)
        {
            Logger.Instance.LogInfo($"Cast Ultimate ServerRpc called by {clientId}");

            Debug.Log("HONEYBEE ULTIMATE: Casting");
            ResetUltimateCharge();

            // Find all game objects with the tag "Player"
            players = GameObject.FindGameObjectsWithTag("Player");
            // Iterate through each player
            foreach (var player in players)
            {
                // Access the PlayerController script
                PlayerController playerController = player.GetComponent<PlayerController>();

                // Check if the playerController is not null and is an ally
                if (playerController != null && playerController.teamId.Value == transform.root.transform.GetComponent<PlayerController>().teamId.Value)
                {
                    // Multiply the MoveSpeed variable by 2
                    playerController.AdjustMovementSpeedServerRpc(playerController.defaultMoveSpeed * 2f);
                    // Reduce damage taken by 25%
                    playerController.AdjustDamageTakenScaleServerRpc(playerController.defaultDamageTakenScale * 0.75f);

                    playerController.passiveHealthRegenerationPercentage = playerController.defaultPassiveHealthRegenerationPercentage + 0.05f;
                }
            }

            ulted = true;
            countdownTimer = armVariable.ultimateDuration;

            //Audio Player
            int attackTypeIndex = 2; //Basic - 0; Skill - 1; Ultimate - 2;
            CastAttackSFXServerRpc(attackTypeIndex, serverRpcParams);

            // Cast the Ultimate ClientRpc
            CastUltimateClientRpc(new ClientRpcParams                   // REMOVE : This just notifies the client
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            });
        }
    }
}