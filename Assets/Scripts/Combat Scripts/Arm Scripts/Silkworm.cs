using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Silkworm : Arm
{
    [SerializeField] private GameObject ultShootPoint;
    private GameObject spellProjectile;
    private GameObject ultimateProjectile;
    private float nextBasicFireTime = 0f;
    private int skillCharges;
    private int maxSkillCharges;
    private float skillChargeTimer;
    private int maxSkillInstantiations;

    public override void Initialize()
    {
        base.Initialize();
        maxSkillCharges = armVariable.skillMaxCharges;
        skillChargeTimer = armVariable.skillCoolDown;
        maxSkillInstantiations = armVariable.skillMaxInstants;

        skillCharges = maxSkillCharges; // Initialize Arm with the maximum skill Charges first

        UltimateCharge = armVariable.ultimateCharge;

        if (projectiles[1] != null)
        {
            spellProjectile = projectiles[1];
        }

        if (projectiles[2] != null)
        {
            ultimateProjectile = projectiles[2];
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
                Debug.Log("Increment Silkworm Skill Charge: " + skillCharges);
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
            Logger.Instance.LogInfo($"Cast Basic Attack ServerRpc called by {clientId}");

            // Instantiate the Projectile
            GameObject shotBasicProjectileClone = Instantiate(basicProjectile, shootPoint.transform.position, transform.rotation);
            shotBasicProjectileClone.layer = transform.root.gameObject.layer;
            // Setup teamId
            shotBasicProjectileClone.GetComponent<Projectile>().teamId.Value = transform.root.transform.GetComponent<PlayerController>().teamId.Value;
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


    // Declare a Queue to track active spellProjectiles
    [System.NonSerialized]
    public List<GameObject> activeSpellProjectiles = new List<GameObject>();

    [ServerRpc(RequireOwnership = false)]
    public override void CastSkillServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;

        if (OwnerClientId != clientId) return;


        if (skillCharges > 0)
        {
            // Check if the number of skill instantiations exceeds the maximum
            if (activeSpellProjectiles.Count >= maxSkillInstantiations)
            {
                // Remove the oldest skillProjectile
                GameObject oldestProjectile = activeSpellProjectiles[0];
                oldestProjectile.transform.GetComponent<NetworkObject>().Despawn(true);
                Destroy(oldestProjectile);

                activeSpellProjectiles.RemoveAt(0);
                Debug.Log("Removed oldest Silkworm skillProjectile");
            }

            // Instantiate the skill projectile and add it to the active projectiles queue
            GameObject skillProjectile = Instantiate(spellProjectile, shootPoint.transform.position, transform.rotation);
            skillProjectile.layer = transform.root.gameObject.layer;
            // Setup teamId
            skillProjectile.GetComponent<SkillObject>().teamId.Value = transform.root.transform.GetComponent<PlayerController>().teamId.Value;
            skillProjectile.transform.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
            activeSpellProjectiles.Add(skillProjectile);
            // Set the instantiatingArm
            skillProjectile.GetComponent<SkillObject>().instantiatingArm = gameObject.GetComponent<Arm>();

            // Decrease the number of available skill charges
            skillCharges--;
            Debug.Log("Decrease Silkworm Skill Charge: " + skillCharges);
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

            Debug.Log("SILKWORM ULTIMATE: Casting");
            UltimateCharge = 0f; // Reset Ultimate Charge
                                 // Instantiate the ultimate area effect
            GameObject ultimateArea = Instantiate(ultimateProjectile, ultShootPoint.transform.position, transform.rotation);
            ultimateArea.layer = transform.root.gameObject.layer;
            // Setup teamId
            ultimateArea.GetComponent<SkillObject>().teamId.Value = transform.root.transform.GetComponent<PlayerController>().teamId.Value;
            ultimateArea.transform.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
            ultimateArea.GetComponent<SkillObject>().instantiatingArm = gameObject.GetComponent<Arm>();
            ultimateArea.GetComponent<SilkRoad>().countdownTimer = armVariable.ultimateDuration;


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