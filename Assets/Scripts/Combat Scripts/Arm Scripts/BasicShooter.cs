using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BasicShooter : Arm
{
    private void Update()
    {
        UpdateWeaponAnimator();
    }

    public override void SetProjectiles()
    {
        basicProjectile = projectiles[0];
    }

    [ServerRpc(RequireOwnership = false)]
    public override void CastBasicAttackServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;

        if (OwnerClientId != clientId) return;

        if (Time.time >= nextBasicFireTime)
        {
            Logger.Instance.LogInfo($"Cast Basic Attack ServerRpc called by {clientId} with layer: {transform.root.gameObject.layer}");

            ShakeCamera();

            GameObject basicProjectileClone = SpawnProjectile<Projectile>(clientId, basicProjectile, shootPoint);
            basicProjectileClone.GetComponent<Projectile>().MaxDistance = 20f;
            FireProjectile(basicProjectileClone);

            CastBasicAttackSFX();                           // TODO: Add this in Arms so that everyone has
            UpdateWeaponState(WeaponState.BasicAttack);     // TODO: Add this to Arms so that everyone has

            CastBasicAttackClientRpc(new ClientRpcParams    // REMOVE :This just notifies the client
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            });

            nextBasicFireTime = Time.time + armVariable.baseFireRate;
        }
        else
        {
            // TODO: This should be like indication if Player can shoot
            // UpdateWeaponState(WeaponState.Idle);
        }

    }

    public void CastSkill()
    {
        // Implement the BasicArm's skill
        // Debug.Log("Casting " + armVariable.armName + "'s Skill with damage: " + spellProjectile.GetComponent<Projectile>().Damage);
        GameObject shotSpellProjectile = Instantiate(spellProjectile, shootPoint.transform.position, transform.rotation);
    }

    public void CastUltimate()
    {
        // Implement the BasicArm's ultimate skill
        // Debug.Log("Casting " + armVariable.armName + "'s Ultimate with damage: " + armVariable.ultimateDamage);
        GameObject shotUltimateProjectile = Instantiate(ultimateProjectile, shootPoint.transform.position, transform.rotation);
    }
}

