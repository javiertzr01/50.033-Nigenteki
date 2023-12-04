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

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        UpdateWeaponAnimator();
    }


    [ServerRpc(RequireOwnership = false)]
    public override void CastBasicAttackServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;

        if (OwnerClientId != clientId) return;

        if (Time.time >= nextBasicFireTime)
        {
            Logger.Instance.LogInfo($"Cast Basic Attack ServerRpc called by {clientId} with layer: {transform.root.gameObject.layer}");

            NetworkManager.Singleton.ConnectedClients[OwnerClientId].PlayerObject.GetComponent<PlayerController>().ShakeCameraClientRpc(basicAttackCameraShakeIntensity, basicAttackCameraShakeDuration, new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            });

            GameObject firedBasicProjectileClone = Instantiate(basicProjectile, shootPoint.transform.position, transform.rotation);
            firedBasicProjectileClone.layer = transform.root.gameObject.layer;
            // Setup teamId
            firedBasicProjectileClone.GetComponent<Projectile>().teamId.Value = transform.root.transform.GetComponent<PlayerController>().teamId.Value;
            firedBasicProjectileClone.transform.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
            firedBasicProjectileClone.GetComponent<Projectile>().instantiatingArm = gameObject.GetComponent<Arm>();
            firedBasicProjectileClone.GetComponent<Projectile>().MaxDistance = 20f;
            Rigidbody2D rb = firedBasicProjectileClone.GetComponent<Rigidbody2D>();
            rb.AddForce(shootPoint.transform.up * armVariable.baseForce, ForceMode2D.Impulse);

            //Audio Player
            int attackTypeIndex = 0; //Basic - 0; Skill - 1; Ultimate - 2;
            CastAttackSFXServerRpc(attackTypeIndex, serverRpcParams);

            UpdateWeaponAnimatorServerRpc(WeaponState.BasicAttack);

            CastBasicAttackClientRpc(new ClientRpcParams
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
            UpdateWeaponAnimatorServerRpc(WeaponState.Idle);
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

        if (Time.time >= nextBasicFireTime)
        {
            Logger.Instance.LogInfo($"Cast Skill ServerRpc called by {clientId} with layer: {transform.root.gameObject.layer}");

            NetworkManager.Singleton.ConnectedClients[OwnerClientId].PlayerObject.GetComponent<PlayerController>().ShakeCameraClientRpc(basicAttackCameraShakeIntensity, basicAttackCameraShakeDuration, new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            });

            GameObject firedBasicProjectileClone = Instantiate(spellProjectile, shootPoint.transform.position, transform.rotation);
            firedBasicProjectileClone.layer = transform.root.gameObject.layer;
            // Setup teamId
            firedBasicProjectileClone.GetComponent<Projectile>().teamId.Value = transform.root.transform.GetComponent<PlayerController>().teamId.Value;
            firedBasicProjectileClone.transform.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
            firedBasicProjectileClone.GetComponent<Projectile>().instantiatingArm = gameObject.GetComponent<Arm>();
            firedBasicProjectileClone.GetComponent<Projectile>().MaxDistance = 20f;
            Rigidbody2D rb = firedBasicProjectileClone.GetComponent<Rigidbody2D>();
            rb.AddForce(shootPoint.transform.up * armVariable.baseForce, ForceMode2D.Impulse);

            //Audio Player
            int attackTypeIndex = 1; //Basic - 0; Skill - 1; Ultimate - 2;
            CastAttackSFXServerRpc(attackTypeIndex, serverRpcParams);

            UpdateWeaponAnimatorServerRpc(WeaponState.BasicAttack);

            CastSkillClientRpc(new ClientRpcParams
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
            UpdateWeaponAnimatorServerRpc(WeaponState.Idle);
        }

    }

    [ClientRpc]
    public override void CastSkillClientRpc(ClientRpcParams clientRpcParams = default)
    {
        if (!IsOwner) return;

        Logger.Instance.LogInfo($"Cast Skills ClientRpc called by {OwnerClientId}");
    }


    [ServerRpc(RequireOwnership = false)]
    public override void CastUltimateServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;

        if (OwnerClientId != clientId) return;

        if (Time.time >= nextBasicFireTime)
        {
            Logger.Instance.LogInfo($"Cast Ultimate Attack ServerRpc called by {clientId} with layer: {transform.root.gameObject.layer}");

            NetworkManager.Singleton.ConnectedClients[OwnerClientId].PlayerObject.GetComponent<PlayerController>().ShakeCameraClientRpc(basicAttackCameraShakeIntensity, basicAttackCameraShakeDuration, new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            });

            GameObject firedBasicProjectileClone = Instantiate(ultimateProjectile, shootPoint.transform.position, transform.rotation);
            firedBasicProjectileClone.layer = transform.root.gameObject.layer;
            // Setup teamId
            firedBasicProjectileClone.GetComponent<Projectile>().teamId.Value = transform.root.transform.GetComponent<PlayerController>().teamId.Value;
            firedBasicProjectileClone.transform.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
            firedBasicProjectileClone.GetComponent<Projectile>().instantiatingArm = gameObject.GetComponent<Arm>();
            firedBasicProjectileClone.GetComponent<Projectile>().MaxDistance = 20f;
            Rigidbody2D rb = firedBasicProjectileClone.GetComponent<Rigidbody2D>();
            rb.AddForce(shootPoint.transform.up * armVariable.baseForce, ForceMode2D.Impulse);

            //Audio Player
            int attackTypeIndex = 2; //Basic - 0; Skill - 1; Ultimate - 2;
            CastAttackSFXServerRpc(attackTypeIndex, serverRpcParams);

            UpdateWeaponAnimatorServerRpc(WeaponState.BasicAttack);

            CastUltimateClientRpc(new ClientRpcParams
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
            UpdateWeaponAnimatorServerRpc(WeaponState.Idle);
        }

    }

    [ClientRpc]
    public override void CastUltimateClientRpc(ClientRpcParams clientRpcParams = default)
    {
        if (!IsOwner) return;

        Logger.Instance.LogInfo($"Cast Ultimate ClientRpc called by {OwnerClientId}");
    }


}

