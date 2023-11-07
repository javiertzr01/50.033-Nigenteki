using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BasicShooter : Arm
{
    protected GameObject spellProjectile;
    protected GameObject ultimateProjectile;

    private float nextBasicFireTime = 0f;

    public override void Initialize()
    {
        base.Initialize();
        // Initialize arm with the variables from armVariable.
        // E.g. attack power, etc.

        if (projectiles[1] != null)
        {
            spellProjectile = projectiles[1];
        }

        if (projectiles[2] != null)
        {
            ultimateProjectile = projectiles[2];
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

            GameObject firedBasicProjectileClone = Instantiate(basicProjectile, shootPoint.transform.position, transform.rotation);
            firedBasicProjectileClone.transform.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
            firedBasicProjectileClone.GetComponent<Projectile>().maxDistance = 20f;
            Rigidbody2D rb = firedBasicProjectileClone.GetComponent<Rigidbody2D>();
            rb.AddForce(shootPoint.transform.up * armVariable.baseForce, ForceMode2D.Impulse);

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

    [ClientRpc]
    public override void CastBasicAttackClientRpc(ClientRpcParams clientRpcParams = default)
    {
        if (!IsOwner) return;

        Logger.Instance.LogInfo($"Cast Basic Attack ClientRpc called by {OwnerClientId}");
    }


    public override void CastSkill()
    {
        // Implement the BasicArm's skill
        // Debug.Log("Casting " + armVariable.armName + "'s Skill with damage: " + spellProjectile.GetComponent<Projectile>().Damage);
        GameObject shotSpellProjectile = Instantiate(spellProjectile, shootPoint.transform.position, transform.rotation);
    }

    public override void CastUltimate()
    {
        // Implement the BasicArm's ultimate skill
        // Debug.Log("Casting " + armVariable.armName + "'s Ultimate with damage: " + armVariable.ultimateDamage);
        GameObject shotUltimateProjectile = Instantiate(ultimateProjectile, shootPoint.transform.position, transform.rotation);
    }
}

