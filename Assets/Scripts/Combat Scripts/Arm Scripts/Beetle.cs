using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;
using Unity.VisualScripting;

public class Beetle : Arm
{
    // Variables (Combat)
    public GameObject currentShield;
    public BeetleShieldTrigger beetleShieldTrigger;
    [SerializeField]
    protected GameObject ultShootPoint;
    protected GameObject shotSpellProjectile;       // For use in CastSkill()
    private bool ulted = false;
    private bool shieldInitialized = false;
    private bool skillReady = true;

    public void Update()
    {
        if (currentShield != null)
        {
            // Attach the Shield to the shootPoint
            currentShield.transform.position = shootPoint.transform.position;
        }

        if(!skillReady)
        {
            if (SkillCoolDown > 0.0f)
            {
                SkillCoolDown -= Time.deltaTime;
            }
            else
            {
                skillReady = true;
            }
        }
        // Check if the ultimate ability is active and if the duration has passed
        if (ulted)
        {
            if (countdownTimer > 0f)
            {
                countdownTimer -= Time.deltaTime;
            }
            else
            {
                ulted = false; // Reset the ulted flag after the ultimate duration
                Debug.Log("BEETLE ULTIMATE: Expired");
                Logger.Instance.LogInfo("BEETLE ULTIMATE: Expired");
            }
        }
    }

    public override void SetProjectiles()
    {
        basicProjectile = projectiles[0];
        spellProjectile = projectiles[1];
        ultimateProjectile = projectiles[2];
        altProjectile = projectiles[3];
    }

    public override void OnUpgraded()
    {
        base.OnUpgraded();
        beetleShieldTrigger.shieldMaxHealth = armVariable.shieldMaxHealthUpgraded;
        beetleShieldTrigger.ShieldHealth = beetleShieldTrigger.shieldMaxHealth;
    }

    public override void OnMax()
    {
        base.OnMax();
        transform.GetComponentInParent<PlayerController>().AdjustPlayerMaxHealthServerRpc(1.5f * transform.GetComponentInParent<PlayerController>().playerMaxHealth.Value);
        transform.GetComponentInParent<PlayerController>().AdjustPlayerHealthServerRpc(transform.GetComponentInParent<PlayerController>().playerMaxHealth.Value);
    }
    // SPAWNING
    [ServerRpc(RequireOwnership = false)]
    private void SpawnShieldServerRpc(ServerRpcParams serverRpcParams = default)
    {
        if (shieldInitialized) return;

        Logger.Instance.LogInfo($"Spawning Shield on {OwnerClientId}");

        // Helpful for if the shield spawns wrongly for client side again
        // Logger.Instance.LogInfo($"Parent Transform {transform.parent.parent.transform.position} + Shield Transform {shieldClone.transform.position}");
        currentShield = SpawnProjectile<ShieldTrigger>(OwnerClientId, basicProjectile, shootPoint);
        currentShield.transform.GetComponent<NetworkObject>().TrySetParent(gameObject.transform);
        beetleShieldTrigger = currentShield.GetComponent<BeetleShieldTrigger>();
        shieldInitialized = true;

        SpawnShieldClientRpc(currentShield.GetComponent<NetworkObject>().NetworkObjectId);
    }

    [ClientRpc]
    public void SpawnShieldClientRpc(ulong shieldId)
    {
        currentShield = NetworkManager.Singleton.SpawnManager.SpawnedObjects[shieldId].gameObject;
        beetleShieldTrigger = currentShield.GetComponent<BeetleShieldTrigger>();
        Logger.Instance.LogInfo($"Spawned Shield on {OwnerClientId}");
    }


// COMBAT
    [ServerRpc(RequireOwnership = false)]
    public override void CastBasicAttackServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;

        if (OwnerClientId != clientId) return;

        if (!shieldInitialized)
        {
            SpawnShieldServerRpc();
        }

        if (ulted)
        {
            // Handle ultimate-specific behavior here
            // For example, instantiate ultimate projectiles instead of the regular ones
            if (ultimateProjectile != null && Time.time >= nextBasicFireTime)
            {
                GameObject shotUltimateProjectile = SpawnProjectile<Projectile>(clientId, ultimateProjectile, ultShootPoint);

                FireProjectile(shotUltimateProjectile, armVariable.ultimateForce);

                nextBasicFireTime = Time.time + armVariable.ultimateFireRate;

                Debug.Log("Casting " + armVariable.armName + "'s Ultimate Attack: ");

                CastBasicAttackClientRpc(new ClientRpcParams                // REMOVE : This just notifies the client
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[] { clientId }
                    }
                });
            }
        }
        else if (beetleShieldTrigger.Destroyed)
        {
            if (altProjectile != null && Time.time >= nextBasicFireTime)
            {
                GameObject noShieldProjectile = SpawnProjectile<Projectile>(clientId, altProjectile, ultShootPoint);

                FireProjectile(noShieldProjectile, armVariable.baseForce);

                nextBasicFireTime = Time.time + armVariable.baseFireRate;

                beetleShieldTrigger.isAttackingNoShield.Value = true;

                Debug.Log("Casting " + armVariable.armName + "'s Alt Attack");

                CastBasicAttackClientRpc(new ClientRpcParams                // REMOVE : This just notifies the client
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[] { clientId }
                    }
                });
            }
        }
        else
        {
            if (Time.time >= nextBasicFireTime)
            {
                beetleShieldTrigger.ToggleShield();
                nextBasicFireTime = Time.time + armVariable.baseFireRate;
            }
        }


    }

    [ServerRpc(RequireOwnership = false)]
    public override void CastSkillServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;

        if (OwnerClientId != clientId) return;

        if (shotSpellProjectile == null && skillReady)
        {
            Debug.Log("BEETLE SKILL: Casting");

            shotSpellProjectile = SpawnProjectile<ShieldTrigger>(clientId, spellProjectile, shootPoint);
            shotSpellProjectile.GetComponent<BeetleSkillTrigger>().ShieldHealth = armVariable.skillForce;
            // Set the skill cooldown to initial value
            SkillCoolDown = armVariable.skillCoolDown;
            skillReady = false;

            // Cast the Skill ClientRpc
            CastSkillClientRpc(new ClientRpcParams                      // REMOVE : This just notifies the client
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            });
        }
        else
        {
            Debug.Log("BEETLE SKILL: Cannot cast yet");
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
            Debug.Log("BEETLE ULTIMATE: Casting");
            ulted = true;
            ResetUltimateCharge();

            // Toggle the shield if it is on
            if (beetleShieldTrigger.isShieldActive.Value)
            {
                beetleShieldTrigger.ToggleShield();
            }

            // Set the start time of the ultimate
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
        else
        {
            Debug.Log("BEETLE ULTIMATE: Not enough Ult Charge");
            Logger.Instance.LogInfo($"Cast Ult ClientRpc called by {OwnerClientId}: FAIL - Charge");
        }


    }
}