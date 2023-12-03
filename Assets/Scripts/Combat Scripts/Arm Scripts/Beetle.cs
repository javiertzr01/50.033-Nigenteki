using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class Beetle : Arm
{
    // Variables
    private GameObject arm;
    [SerializeField]
    private GameObject shield;

    // Variables (Combat)
    public GameObject currentShield;
    public BeetleShieldTrigger beetleShieldTrigger;
    [SerializeField]
    protected GameObject ultShootPoint;
    protected GameObject spellProjectile;
    protected GameObject ultimateProjectile;
    protected GameObject altProjectile;
    protected GameObject shotSpellProjectile;       // For use in CastSkill()
    private bool ulted;
    private float nextBasicFireTime = 0f;           // for alt fire
    private bool shieldInitialized = false;

    // Properties
    public float ultimateStartTime { get; private set; }
    


    public override void Initialize()
    {
        base.Initialize();
        // Initialize arm with the variables from armVariable.
        // E.g. attack power, etc.

        // Instantiate the shield but hide it and its collider first
        SpawnShieldServerRpc();
        shield.GetComponent<ShieldTrigger>().instantiatingArm = gameObject;
        beetleShieldTrigger = shield.GetComponent<BeetleShieldTrigger>();

        SkillCoolDown = 0f; // Set skill cooldown to zero initially

        UltimateCharge = armVariable.ultimateCharge;
        ulted = false;

        beetleShieldTrigger.ToggleShieldServerRpc();



        if (projectiles[1] != null)
        {
            spellProjectile = projectiles[1];
        }

        if (projectiles[2] != null)
        {
            ultimateProjectile = projectiles[2];
        }

        if (projectiles[3] != null)
        {
            altProjectile = projectiles[3];
        }

    }

    public void Update()
    {
        if (currentShield != null)
        {
            // Attach the Shield to the shootPoint
            currentShield.transform.position = shootPoint.transform.position;
        }

        if (SkillCoolDown > 0.0f)
        {
            SkillCoolDown -= Time.deltaTime;
        }

        // Check if the ultimate ability is active and if the duration has passed
        if (ulted && (Time.time - ultimateStartTime) >= armVariable.ultimateDuration)
        {
            ulted = false; // Reset the ulted flag after the ultimate duration
            Debug.Log("BEETLE ULTIMATE: Expired");
            Logger.Instance.LogInfo("BEETLE ULTIMATE: Expired");

        }
    }



// SPAWNING
    [ServerRpc(RequireOwnership = false)]
    private void SpawnShieldServerRpc(ServerRpcParams serverRpcParams = default)
    {
        if (shieldInitialized) return;

        Logger.Instance.LogInfo($"Spawning Shield on {OwnerClientId}");

        arm = this.gameObject;

        GameObject shieldClone = Instantiate(basicProjectile, shootPoint.transform.position, shootPoint.transform.rotation);
        shieldClone.layer = transform.root.gameObject.layer;
        shieldClone.GetComponent<ShieldTrigger>().teamId.Value = transform.root.transform.GetComponent<PlayerController>().teamId.Value;
        shieldClone.transform.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
        shieldClone.transform.GetComponent<NetworkObject>().TrySetParent(arm.transform);
        shield = shieldClone;

        shieldInitialized = true;

        SpawnShieldClientRpc(new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { OwnerClientId }
            }
        });

    }

    [ClientRpc]
    public void SpawnShieldClientRpc(ClientRpcParams clientRpcParams = default)
    {
        Logger.Instance.LogInfo($"Spawned Shield on {OwnerClientId}");
    }



// COMBAT
    [ServerRpc(RequireOwnership = false)]
    public override void CastBasicAttackServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;

        if (OwnerClientId != clientId) return;

        if (ulted)
        {
            // Handle ultimate-specific behavior here
            // For example, instantiate ultimate projectiles instead of the regular ones
            if (ultimateProjectile != null && Time.time >= nextBasicFireTime)
            {
                GameObject shotUltimateProjectile = Instantiate(ultimateProjectile, ultShootPoint.transform.position, transform.rotation);
                shotUltimateProjectile.layer = transform.root.gameObject.layer;
                // Setup teamId
                shotUltimateProjectile.GetComponent<Projectile>().teamId.Value = transform.root.transform.GetComponent<PlayerController>().teamId.Value;
                shotUltimateProjectile.transform.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
                shotUltimateProjectile.GetComponent<Projectile>().instantiatingArm = gameObject.GetComponent<Arm>();
                Rigidbody2D rb = shotUltimateProjectile.GetComponent<Rigidbody2D>();
                rb.AddForce(ultShootPoint.transform.up * armVariable.ultimateForce, ForceMode2D.Impulse);
                Debug.Log("Casting " + armVariable.armName + "'s Ultimate Attack: ");

                CastBasicAttackClientRpc(new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[] { clientId }
                    }
                });

                nextBasicFireTime = Time.time + armVariable.ultimateFireRate;
            }
        }
        else if (beetleShieldTrigger.Destroyed)
        {
            if (altProjectile != null && Time.time >= nextBasicFireTime)
            {
                GameObject shotBasicProjectile = Instantiate(altProjectile, ultShootPoint.transform.position, transform.rotation);
                shotBasicProjectile.layer = transform.root.gameObject.layer;
                // Setup teamId
                shotBasicProjectile.GetComponent<Projectile>().teamId.Value = transform.root.transform.GetComponent<PlayerController>().teamId.Value;
                shotBasicProjectile.transform.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
                shotBasicProjectile.GetComponent<Projectile>().instantiatingArm = gameObject.GetComponent<Arm>();
                Rigidbody2D rb = shotBasicProjectile.GetComponent<Rigidbody2D>();
                rb.AddForce(ultShootPoint.transform.up * armVariable.baseForce, ForceMode2D.Impulse);
                Debug.Log("Casting " + armVariable.armName + "'s Alt Attack");

                CastBasicAttackClientRpc(new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[] { clientId }
                    }
                });

                nextBasicFireTime = Time.time + armVariable.baseFireRate;
            }
        }
        else
        {
            if (Time.time >= nextBasicFireTime)
            {
                beetleShieldTrigger.ToggleShieldServerRpc();
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

        if (shotSpellProjectile == null && SkillCoolDown <= 0.0f)
        {
            Debug.Log("BEETLE SKILL: Casting");
            shotSpellProjectile = Instantiate(spellProjectile, shootPoint.transform.position, transform.rotation);
            shotSpellProjectile.layer = transform.root.gameObject.layer;
            // Setup teamId
            shotSpellProjectile.GetComponent<ShieldTrigger>().teamId.Value = transform.root.transform.GetComponent<PlayerController>().teamId.Value;
            shotSpellProjectile.GetComponent<BeetleSkillTrigger>().ShieldHealth = armVariable.skillForce;
            shotSpellProjectile.GetComponent<ShieldTrigger>().instantiatingArm = gameObject;
            shotSpellProjectile.transform.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
            // Destroy(shotSpellProjectile, armVariable.skillDuration);

            // Set the skill cooldown to initial value
            SkillCoolDown = armVariable.skillCoolDown;

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
            Debug.Log("BEETLE SKILL: Cannot cast yet");
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
            Debug.Log("BEETLE ULTIMATE: Casting");
            ulted = true;
            UltimateCharge = 0f; // Reset Ultimate Charge

            // Toggle the shield if it is on
            if (beetleShieldTrigger.isShieldActive.Value)
            {
                beetleShieldTrigger.ToggleShieldServerRpc();
            }

            // Set the start time of the ultimate
            ultimateStartTime = Time.time;

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

    [ClientRpc]
    public override void CastUltimateClientRpc(ClientRpcParams clientRpcParams = default)
    {
        if (!IsOwner) return;
        Logger.Instance.LogInfo($"Cast Ultimate ClientRpc called by {OwnerClientId}");
    }

}