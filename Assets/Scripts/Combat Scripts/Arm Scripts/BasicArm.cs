using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BasicArm : Arm
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

    /*
    public override void CastBasicAttack()
    {

        if (Time.time >= nextBasicFireTime)
        {
            // Implement the BasicArm's basic attack
            Debug.Log("Casting " + armVariable.armName + "'s Basic Attack with damage: " + armVariable.baseDamage);
            GameObject firedBasicProjectile = Instantiate(basicProjectile, shootPoint.transform.position, transform.rotation);
            firedBasicProjectile.transform.GetComponent<NetworkObject>().Spawn(true);
            firedBasicProjectile.GetComponent<Projectile>().maxDistance = 20f;
            Rigidbody2D rb = firedBasicProjectile.GetComponent<Rigidbody2D>();
            rb.AddForce(shootPoint.transform.up * armVariable.baseForce, ForceMode2D.Impulse);

            nextBasicFireTime = Time.time + armVariable.baseFireRate;
        }
    }*/
    [ServerRpc]
    public override void CastBasicAttackServerRpc()
    {

        if(Time.time >= nextBasicFireTime)
        {
            // Implement the BasicArm's basic attack
            Debug.Log("Casting " + armVariable.armName + "'s Basic Attack with damage: " + armVariable.baseDamage);
            GameObject firedBasicProjectile = Instantiate(basicProjectile, shootPoint.transform.position, transform.rotation);
            firedBasicProjectile.transform.GetComponent<NetworkObject>().Spawn(true);
            firedBasicProjectile.GetComponent<Projectile>().maxDistance = 20f;
            Rigidbody2D rb = firedBasicProjectile.GetComponent<Rigidbody2D>();
            rb.AddForce(shootPoint.transform.up * armVariable.baseForce, ForceMode2D.Impulse);

            nextBasicFireTime = Time.time + armVariable.baseFireRate;
        }
    }
    [ClientRpc]
    public override void CastBasicAttackClientRpc()
    {
        throw new System.NotImplementedException();
    }

    public override void CastSkill()
    {
        // Implement the BasicArm's skill
        Debug.Log("Casting " + armVariable.armName + "'s Skill with damage: " + armVariable.skillDamage);
        GameObject shotSpellProjectile = Instantiate(spellProjectile, shootPoint.transform.position, transform.rotation);
    }

    public override void CastUltimate()
    {
        // Implement the BasicArm's ultimate skill
        Debug.Log("Casting " + armVariable.armName + "'s Ultimate with damage: " + armVariable.ultimateDamage);
        GameObject shotUltimateProjectile = Instantiate(ultimateProjectile, shootPoint.transform.position, transform.rotation);
    }
}

