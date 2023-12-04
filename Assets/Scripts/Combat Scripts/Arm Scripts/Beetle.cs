using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class Beetle : Arm
{
    [SerializeField]
    protected GameObject ultShootPoint;
    protected GameObject spellProjectile;
    protected GameObject ultimateProjectile;
    protected GameObject altProjectile;
    // private float _shieldCurrentHealth;
    // private float shieldRegenTimer;
    // protected bool activated;
    // protected bool destroyed;
    public GameObject currentShield;
    public BeetleShieldTrigger beetleShieldTrigger;
    protected GameObject shotSpellProjectile;     // For use in CastSkill()
    private float nextBasicFireTime = 0f; // for alt fire
    private bool ulted;
    private GameObject arm;
    [SerializeField]
    private GameObject shieldHolderPrefab;
    private GameObject shieldHolder;
    private GameObject shield;
    private bool shieldInitialized = false;

    [ServerRpc(RequireOwnership = false)]
    private void SpawnShieldServerRpc(ServerRpcParams serverRpcParams = default)
    {
        if (shieldInitialized) return;

        Logger.Instance.LogInfo($"Spawning Shield on {OwnerClientId}");

        arm = this.gameObject;

        // GameObject shieldHolderClone = Instantiate(shieldHolderPrefab, arm.transform.GetComponent<NetworkObject>().transform.position + shieldHolderPrefab.transform.localPosition, Quaternion.Euler(0, 0, 0));
        // shieldHolderClone.transform.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
        // shieldHolderClone.GetComponent<NetworkObject>().TrySetParent(arm.transform);
        // shieldHolder = shieldHolderClone;

        GameObject shieldClone = Instantiate(basicProjectile, shootPoint.transform.position, shootPoint.transform.rotation);
        shieldClone.layer = transform.root.gameObject.layer;
        shieldClone.GetComponent<ShieldTrigger>().teamId.Value = transform.root.transform.GetComponent<PlayerController>().teamId.Value;
        shieldClone.transform.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
        shieldClone.transform.GetComponent<NetworkObject>().TrySetParent(arm.transform);
        shield = shieldClone;

        shieldInitialized = true;

        ////Audio Player
        //int attackTypeIndex = 0; //Basic - 0; Skill - 1; Ultimate - 2;
        //CastAttackSFXServerRpc(attackTypeIndex, serverRpcParams);

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


    public float ultimateStartTime { get; private set; }

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

                //Audio Player
                int attackTypeIndex = 0; //Basic - 0; Skill - 1; Ultimate - 2;
                CastAttackSFXServerRpc(attackTypeIndex, serverRpcParams);

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

                //Audio Player
                // DisabledSFX
                int attackTypeIndex = 2; //Basic - 0; Skill - 1; Ultimate - 2;
                CastAttackSFXServerRpc(attackTypeIndex, serverRpcParams);

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

            //Audio Player
            int attackTypeIndex = 1; //Basic - 0; Skill - 1; Ultimate - 2;
            CastAttackSFXServerRpc(attackTypeIndex, serverRpcParams);

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

            ////Audio Player
            //int attackTypeIndex = 2; //Basic - 0; Skill - 1; Ultimate - 2;
            //CastAttackSFXServerRpc(attackTypeIndex, serverRpcParams);

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