using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Locust : Arm
{
    GameObject ProjectileSpawned;
    [SerializeField]
    private bool ulted;

    public override void Initialize()
    {
        base.Initialize();

        ulted = false;
    }

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
                Debug.Log("Increment Locust Skill Charge: " + SkillCharges);
                // Reset the skill charge timer
                SkillCoolDown = armVariable.skillCoolDown;
            }
        }
        // Update the ultimate timer
        if (ulted)
        {
            countdownTimer -= Time.deltaTime;
            if (countdownTimer <= 0f)
            {
                ulted = false; // Reset the ulted flag when the timer reaches 0
                gameObject.GetComponentInParent<PlayerController>().ToggleImmuneStunServerRpc(false);
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
            if (ulted || ProjectileSpawned == null)
            {
                Logger.Instance.LogInfo($"Cast Basic Attack ServerRpc called by {clientId}");

                // Instantiate the type of Projectile
                if (ulted)
                {
                    ProjectileSpawned = SpawnProjectile<Projectile>(clientId, ultimateProjectile, shootPoint);

                }
                else
                {
                    ProjectileSpawned = SpawnProjectile<Projectile>(clientId, basicProjectile, shootPoint);
                }
                FireProjectile(ProjectileSpawned);

                // Set the nextBasicFireTime
                if (ulted)
                {
                    nextBasicFireTime = Time.time + armVariable.ultimateFireRate;
                }
                else
                {
                    nextBasicFireTime = Time.time + armVariable.baseFireRate;
                }

                CastBasicAttackClientRpc(new ClientRpcParams        // REMOVE : This just notifies the client
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[] { clientId }
                    }
                });
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public override void CastSkillServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;

        if (OwnerClientId != clientId) return;
        Debug.Log("Skill casted");

        if (ulted || SkillCharges > 0)
        {
            // Trigger the dash on the client
            gameObject.GetComponentInParent<PlayerController>().TriggerDashClientRpc(new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            });

            // Decrease the number of available skill charges
            if (!ulted)
            {
                SkillCharges--;
                Debug.Log("Decrease Locust Skill Charge: " + SkillCharges);
            }
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

            Debug.Log("LOCUST ULTIMATE: Casting");
            ResetUltimateCharge();
            ulted = true;
            gameObject.GetComponentInParent<PlayerController>().ToggleImmuneStunServerRpc(true);
            // Cast the Ultimate ClientRpc
            CastUltimateClientRpc(new ClientRpcParams               // REMOVE : This just notifies the client
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            });
        }
    }
}