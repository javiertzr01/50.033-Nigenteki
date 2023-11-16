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
            shotBasicProjectileClone.transform.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
            // Set the instantiatingArm
            shotBasicProjectileClone.GetComponent<Projectile>().instantiatingArm = gameObject.GetComponent<Arm>();
            // Set max distance and apply force
            shotBasicProjectileClone.GetComponent<Projectile>().maxDistance = 20f;
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

        Debug.Log($"Attempting to CastSkillServerRpc for client {clientId}");

        if (OwnerClientId != clientId) return;

        Debug.Log($"Executing CastSkillServerRpc for client {clientId}");


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

    public void CastSkill()
    {

    }

    public override void CastUltimate()
    {
        if (UltimateCharge >= 100f)
        {
            Debug.Log("SILKWORM ULTIMATE: Casting");
            UltimateCharge = 0f; // Reset Ultimate Charge
                                 // Instantiate the ultimate area effect
            GameObject ultimateArea = Instantiate(ultimateProjectile, ultShootPoint.transform.position, transform.rotation);


            // Start a timer to destroy the ultimate area after 15 seconds
            StartCoroutine(DestroyUltimateArea(ultimateArea, 15f));
        }
    }

    private IEnumerator DestroyUltimateArea(GameObject area, float duration)
    {
        yield return new WaitForSeconds(duration);

        // Implement logic to apply stun to objects with PlayerController script
        // Modify moveSpeed and attackSpeed as described

        // Destroy the ultimate area after the specified duration
        Destroy(area);
    }
}