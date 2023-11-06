using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Beetle : Arm
{
    [SerializeField]
    protected GameObject ultShootPoint;
    protected GameObject spellProjectile;
    protected GameObject ultimateProjectile;
    protected GameObject altProjectile;
    private float _shieldCurrentHealth;
    private float _skillCoolDown;
    private float shieldRegenTimer;
    protected bool activated;
    protected bool destroyed;
    protected GameObject currentShield;
    protected GameObject shotSpellProjectile;     // For use in CastSkill()
    private float nextBasicFireTime = 0f; // for alt fire
    private bool ulted;



    public override void Initialize()
    {
        base.Initialize();
        // Initialize arm with the variables from armVariable.
        // E.g. attack power, etc.

        // Instantiate the shield but hide it and its collider first
        currentShield = Instantiate(basicProjectile, shootPoint.transform.position, shootPoint.transform.rotation, transform);

        ShieldHealth = armVariable.shieldMaxHealth; // Shield Variable
        SkillCoolDown = 0f; // Set skill cooldown to zero initially
        shieldRegenTimer = 0f; // Initialize shield regen timer to zero

        UltimateCharge = armVariable.ultimateCharge;
        ulted = false;

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

        if (projectiles[3] != null)
        {
            altProjectile = projectiles[3];
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


        // Shield regeneration
        if (!activated)
        {
            shieldRegenTimer += Time.deltaTime;
            if (shieldRegenTimer >= 3.0f) // Regenerate the shield health after 3 seconds of inactivity
            {
                if (ShieldHealth < armVariable.shieldMaxHealth)
                {
                    ShieldHealth += 15f * Time.deltaTime; // Regenerate 15 HP per second
                    Debug.Log("BEETLE SHIELD: Regenerating: " + ShieldHealth);
                    if (ShieldHealth >= armVariable.shieldMaxHealth)
                    {
                        ShieldHealth = armVariable.shieldMaxHealth;
                        destroyed = false; // Reset destroyed flag if the shield is fully regenerated
                        Debug.Log("BEETLE SHIELD: restored");
                    }
                }
            }
        }
        else
        {
            shieldRegenTimer = 0f; // Reset the timer if the shield is activated again
        }


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
        if (ulted)
        {
            // Handle ultimate-specific behavior here
            // For example, instantiate ultimate projectiles instead of the regular ones
            if (ultimateProjectile != null && Time.time >= nextBasicFireTime)
            {
                GameObject shotUltimateProjectile = Instantiate(ultimateProjectile, shootPoint.transform.position, transform.rotation);
                shotUltimateProjectile.GetComponent<Projectile>().instantiatingArm = gameObject;
                Rigidbody2D rb = shotUltimateProjectile.GetComponent<Rigidbody2D>();
                rb.AddForce(ultShootPoint.transform.up * armVariable.ultimateForce, ForceMode2D.Impulse);
                Debug.Log("Casting " + armVariable.armName + "'s Ultimate Attack with damage: " + shotUltimateProjectile.GetComponent<Projectile>().Damage);

                nextBasicFireTime = Time.time + armVariable.ultimateFireRate;
            }
        }
        else if (destroyed)
        {
            if (altProjectile != null && Time.time >= nextBasicFireTime)
            {
                GameObject shotBasicProjectile = Instantiate(altProjectile, shootPoint.transform.position, transform.rotation);
                shotBasicProjectile.GetComponent<Projectile>().instantiatingArm = gameObject;
                Rigidbody2D rb = shotBasicProjectile.GetComponent<Rigidbody2D>();
                rb.AddForce(shootPoint.transform.up * armVariable.baseForce, ForceMode2D.Impulse);
                Debug.Log("Casting " + armVariable.armName + "'s Alt Attack with damage: " + shotBasicProjectile.GetComponent<Projectile>().Damage);

                nextBasicFireTime = Time.time + armVariable.baseFireRate;
            }
        }
        else
        {
            ToggleShield();
        }
    }

    public override void CastSkill()
    {
        if (shotSpellProjectile == null && SkillCoolDown <= 0.0f)
        {
            Debug.Log("BEETLE SKILL: Casting");
            shotSpellProjectile = Instantiate(spellProjectile, shootPoint.transform.position, transform.rotation);
            shotSpellProjectile.GetComponent<ShieldTrigger>().instantiatingArm = gameObject;
            Destroy(shotSpellProjectile, armVariable.skillDuration);

            // Set the skill cooldown to initial value
            SkillCoolDown = armVariable.skillCoolDown;
        }
        else
        {
            Debug.Log("BEETLE SKILL: Cannot cast yet");
        }

    }

    public override void CastUltimate()
    {
        if (UltimateCharge >= 100f)
        {
            Debug.Log("BEETLE ULTIMATE: Casting");
            ulted = true;
            UltimateCharge = 0f; // Reset Ultimate Charge

            // Toggle the shield if it is on
            if (activated)
            {
                ToggleShield();
            }

            // Start a timer for the ultimate's duration
            StartCoroutine(UltimateDurationTimer(armVariable.ultimateDuration));
        }
        else
        {
            Debug.Log("BEETLE ULTIMATE: Not enough Ult Charge");
        }

    }

    // Coroutine to handle the duration of the ultimate
    private IEnumerator UltimateDurationTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        ulted = false; // Reset the ulted flag after the ultimate duration
        Debug.Log("BEETLE ULTIMATE: Expired");
    }

}
