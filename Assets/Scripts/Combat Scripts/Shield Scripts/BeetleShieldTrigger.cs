using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BeetleShieldTrigger : ShieldTrigger
{
    Beetle arm;

    private float _shieldCurrentHealth;
    private float shieldRegenTimer;
    protected bool _activated;
    protected bool _destroyed;

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

    public bool Activated
    {
        get
        {
            return _activated;
        }
        set
        {
            _activated = value;
        }
    }

    public bool Destroyed
    {
        get
        {
            return _destroyed;
        }
        set
        {
            _destroyed = value;
        }
    }

    public void ToggleShield()
    {
        // As long as shield is not destroyed, can keep toggling
        if (!Destroyed)
        {
            Collider2D shieldCollider = gameObject.GetComponent<BoxCollider2D>();
            SpriteRenderer shieldSprite = gameObject.GetComponentInChildren<SpriteRenderer>();
            // Toggle the shield's collider and sprite renderer
            shieldCollider.enabled = !Activated;
            shieldSprite.enabled = !Activated;
            Activated = !Activated;
        }
    }


    void Start()
    {
        shieldRegenTimer = 0f;
        Activated = true;
        Destroyed = false;
        ShieldHealth = instantiatingArm.GetComponent<Arm>().armVariable.shieldMaxHealth;
    }

    void Update()
    {
        // Shield regeneration
        if (!Activated)
        {
            shieldRegenTimer += Time.deltaTime;
            if (shieldRegenTimer >= 3.0f) // Regenerate the shield health after 3 seconds of inactivity
            {
                if (ShieldHealth < instantiatingArm.GetComponent<Arm>().armVariable.shieldMaxHealth)
                {
                    ShieldHealth += 15f * Time.deltaTime; // Regenerate 15 HP per second
                    Debug.Log("BEETLE SHIELD: Regenerating: " + ShieldHealth);
                    if (ShieldHealth >= instantiatingArm.GetComponent<Arm>().armVariable.shieldMaxHealth)
                    {
                        ShieldHealth = instantiatingArm.GetComponent<Arm>().armVariable.shieldMaxHealth;
                        Destroyed = false; // Reset destroyed flag if the shield is fully regenerated
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
        if (ShieldHealth < 0 && !Destroyed)
        {
            ShieldHealth = 0f;
            Destroyed = true;
            Debug.Log("BEETLE SHIELD: destroyed");
            if (Activated)
            {
                Collider2D shieldCollider = gameObject.GetComponent<BoxCollider2D>();
                SpriteRenderer shieldSprite = gameObject.GetComponentInChildren<SpriteRenderer>();
                shieldCollider.enabled = false;
                shieldSprite.enabled = false;
                Activated = false;
            }
        }
    }


    public override void TriggerEnter2DLogic(Collider2D other)
    {
        arm = instantiatingArm.GetComponent<Beetle>();

        // Check if the collision is with a specific object or has specific properties
        if (other.gameObject.TryGetComponent<Projectile>(out Projectile projectile))
        {
            float projectileDamage = projectile.Damage;

            // This is for BasicShield
            Beetle shieldArm = transform.parent.gameObject.GetComponent<Beetle>();

            // Damage the shield
            ShieldHealth -= projectileDamage;
            arm.ChargeUltimate(projectileDamage, 15);


        }
    }
}
