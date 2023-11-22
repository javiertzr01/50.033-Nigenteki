using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;

public class HoneyBee : Arm
{
    private GameObject spellProjectile;
    private float nextBasicFireTime = 0f;
    private bool ulted = false;
    private float countdownTimer = 0f;
    private GameObject[] players;
    private bool skillReady;


    public override void Initialize()
    {
        base.Initialize();

        UltimateCharge = armVariable.ultimateCharge;
        SkillCoolDown = 0f; // Set skill cooldown to zero initially
        skillReady = true;

        // Find all game objects with the tag "Player"
        players = GameObject.FindGameObjectsWithTag("Player");
        // TODO: Add filter for teams


        if (projectiles[1] != null)
        {
            spellProjectile = projectiles[1];
        }

    }
    public void Update()
    {
        if (SkillCoolDown > 0.0f)
        {
            SkillCoolDown -= Time.deltaTime;
        }
        else if (spellProjectile != null && SkillCoolDown <= 0f && !skillReady)
        {
            Debug.Log("Honeybee Skill can be casted");
            skillReady = true;
        }


        if (ulted)
        {
            if (countdownTimer > 0f)
            {
                countdownTimer -= Time.deltaTime;
                if (countdownTimer <= 0f)
                {
                    Debug.Log("Honeybee Ultimate ended");
                    // Reset
                    // Iterate through each player
                    foreach (var player in players)
                    {
                        // Access the PlayerController script
                        PlayerController playerController = player.GetComponent<PlayerController>();

                        // Check if the playerController is not null
                        if (playerController != null)
                        {
                            // Reset the MoveSpeed variable
                            playerController.MoveSpeed /= 2;
                            // Reset damage taken
                            playerController.damageTakenScale /= 0.75f;

                            // TODO: Deactivate Passive Health Regen
                            playerController.passiveHealthRegenerationPercentage -= 0.05f;
                        }
                    }

                    ulted = false;
                    countdownTimer = armVariable.ultimateDuration;
                }
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

            // Instantiate the Projectile
            GameObject shotBasicProjectileClone = Instantiate(basicProjectile, shootPoint.transform.position, transform.rotation);
            shotBasicProjectileClone.transform.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
            // Set the instantiatingArm
            shotBasicProjectileClone.GetComponent<Projectile>().instantiatingArm = gameObject.GetComponent<Arm>();
            // Set max distance and apply force
            // shotBasicProjectileClone.GetComponent<Projectile>().MaxDistance = 20f;
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

        if (spellProjectile != null && SkillCoolDown <= 0f)
        {
            Debug.Log("HONEYBEE SKILL: Casting");
            // Instantiate the skill projectile and add it to the active projectiles queue
            GameObject skillProjectile = Instantiate(spellProjectile, shootPoint.transform.position, transform.rotation);
            skillProjectile.transform.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
            // Set the instantiatingArm
            skillProjectile.GetComponent<SkillObject>().instantiatingArm = gameObject.GetComponent<Arm>();
            skillProjectile.GetComponent<HoneyComb>().countdownTimer = armVariable.skillDuration;

            // Set the skill cooldown to initial value
            SkillCoolDown = armVariable.skillCoolDown;
            skillReady = false;

            // Cast the Skill ClientRpc
            CastSkillClientRpc(new ClientRpcParams
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

            Debug.Log("HONEYBEE ULTIMATE: Casting");
            UltimateCharge = 0f; // Reset Ultimate Charge

            // Iterate through each player
            foreach (var player in players)
            {
                // Access the PlayerController script
                PlayerController playerController = player.GetComponent<PlayerController>();

                // Check if the playerController is not null
                if (playerController != null)
                {
                    // Multiply the MoveSpeed variable by 2
                    playerController.MoveSpeed *= 2;
                    // Reduce damage taken by 25%
                    playerController.damageTakenScale *= 0.75f;

                    // TODO: Activate Passive Health Regen
                    playerController.passiveHealthRegenerationPercentage += 0.05f;
                }
            }

            ulted = true;
            countdownTimer = armVariable.ultimateDuration;


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