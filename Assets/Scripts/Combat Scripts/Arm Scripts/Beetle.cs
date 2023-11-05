using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Beetle : Arm
{
    protected GameObject spellProjectile;
    protected GameObject ultimateProjectile;
    private float _shieldCurrentHealth;
    private float _skillCoolDown;
    protected bool activated;
    protected bool destroyed;
    protected GameObject currentShield;
    protected GameObject shotSpellProjectile;     // For use in CastSkill()



    public override void Initialize()
    {
        base.Initialize();
        // Initialize arm with the variables from armVariable.
        // E.g. attack power, etc.

        // Instantiate the shield but hide it and its collider first
        currentShield = Instantiate(basicProjectile, shootPoint.transform.position, shootPoint.transform.rotation, transform);

        ShieldHealth = armVariable.shieldMaxHealth; // Shield Variable
        SkillCoolDown = 0f; // Set skill cooldown to zero initially

        activated = true;
        destroyed = false;
        ToggleShield(); // Switch shield off first


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


    public float SkillCoolDown
    {
        get
        {
            return _skillCoolDown;
        }
        set
        {
            _skillCoolDown = value;
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


        // TODO: Implement shield regeneration function - after 5 seconds of no activation it will start regenerating health
        // Break shield if shield HP drops to 0 or past 0
        if (ShieldHealth < 0 && !destroyed)
        {
            destroyed = true;
            Debug.Log("BEETLE SHIELD: destroyed");
            if (activated)
            {
                Collider2D shieldCollider = currentShield.GetComponent<BoxCollider2D>();
                SpriteRenderer shieldSprite = currentShield.GetComponentInChildren<SpriteRenderer>();
                shieldCollider.enabled = false;
                shieldSprite.enabled = false;
                activated = false;
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

    public override void CastBasicAttack()
    {
        ToggleShield();
    }

    public override void CastSkill()
    {
        if (shotSpellProjectile == null && SkillCoolDown <= 0.0f)
        {
            Debug.Log("BEETLE SKILL: Casting");
            shotSpellProjectile = Instantiate(spellProjectile, shootPoint.transform.position, transform.rotation);
            Destroy(shotSpellProjectile, armVariable.skillDuration);

            // Set the skill cooldown to initial value
            SkillCoolDown = armVariable.skillCoolDown;
        }
        else
        {
            Debug.Log("BEETLE SKILL; Cannot cast yet");
        }

    }

    public override void CastUltimate()
    {
        // Debug.Log("Casting " + armVariable.armName + "'s Ultimate with damage: " + armVariable.ultimateDamage);
        GameObject shotUltimateProjectile = Instantiate(ultimateProjectile, shootPoint.transform.position, transform.rotation);
    }
}
