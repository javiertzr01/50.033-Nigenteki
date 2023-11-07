using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BasicShield : Arm
{
    protected GameObject spellProjectile;
    protected GameObject ultimateProjectile;
    private float _shieldCurrentHealth;
    protected bool activated;
    protected bool destroyed;
    protected GameObject currentShield;


    public override void Initialize()
    {
        base.Initialize();
        // Initialize arm with the variables from armVariable.
        // E.g. attack power, etc.

        // Instantiate the shield but hide it and its collider first
        currentShield = Instantiate(basicProjectile, shootPoint.transform.position, shootPoint.transform.rotation, transform);
        // Shield variables
        ShieldHealth = armVariable.shieldMaxHealth;
        activated = true;
        destroyed = false;
        // Switch shield off first
        ToggleShield();

        if (projectiles[1] != null)
        {
            spellProjectile = projectiles[1];
        }

        if (projectiles[2] != null)
        {
            ultimateProjectile = projectiles[2];
        }

    }

    public float ShieldHealth
    {
        get
        {
            return _shieldCurrentHealth;
        }
        set
        {
            _shieldCurrentHealth = value;
        }
    }

    public void Update()
    {
        if (currentShield != null)
        {
            // Attach the Shield to the shootPoint
            currentShield.transform.position = shootPoint.transform.position;
        }


        // TODO: Implement shield regeneration function - after 5 seconds of no activation it will start regenerating health
        // Break shield if shield HP drops to 0 or past 0
        if (ShieldHealth < 0 && !destroyed)
        {
            destroyed = true;
            if (activated)
            {
                ToggleShield();
            }
        }


    }


    private void ToggleShield()
    {
        // As long as shield is not destroyed, can keep toggling
        if (!destroyed)
        {
            Collider2D shieldCollider = currentShield.GetComponent<BoxCollider2D>();
            SpriteRenderer shieldSprite = currentShield.GetComponentInChildren<SpriteRenderer>();
            // Toggle the shield's collider and sprite renderer
            shieldCollider.enabled = !activated;
            shieldSprite.enabled = !activated;
            activated = !activated;
        }
    }





    [ServerRpc(RequireOwnership = false)]
    public override void CastBasicAttackServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;

        if (OwnerClientId != clientId) return;

        Logger.Instance.LogInfo($"Cast Basic Attack ServerRpc called by {clientId}");
        ToggleShield();

        //Debug.Log("Casting " + armVariable.armName + "'s Basic Attack with damage: " + firedBasicProjectile.GetComponent<Projectile>().Damage);


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
        // Debug.Log("Casting " + armVariable.armName + "'s Skill with damage: " + armVariable.skillDamage);
        GameObject shotSpellProjectile = Instantiate(spellProjectile, shootPoint.transform.position, transform.rotation);
    }

    public override void CastUltimate()
    {
        // Implement the BasicArm's ultimate skill
        // Debug.Log("Casting " + armVariable.armName + "'s Ultimate with damage: " + armVariable.ultimateDamage);
        GameObject shotUltimateProjectile = Instantiate(ultimateProjectile, shootPoint.transform.position, transform.rotation);
    }
}
