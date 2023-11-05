using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeetleSkillTrigger : ShieldTrigger
{
    Beetle arm;

    [SerializeField]
    private float shieldHealth;

    void Start()
    {
        arm = instantiatingArm.GetComponent<Beetle>();
    }

    private void DestroyShield()
    {
        Destroy(gameObject);
    }

    public override void TriggerEnter2DLogic(Collider2D other)
    {
        // Check if the collision is with a specific object or has specific properties
        if (other.gameObject.TryGetComponent<Projectile>(out Projectile projectile))
        {
            float projectileDamage = projectile.Damage;

            // Damage the shield
            shieldHealth -= projectileDamage;
            ChargeUltimate(projectileDamage, 15);

            if (shieldHealth <= 0)
            {
                DestroyShield();
            }
        }
    }
    // Higher the Divisor, the slower the charging rate
    void ChargeUltimate(float charge, float divisor)
    {
        if (divisor < 1)
        {
            divisor = 1;
        }
        arm.UltimateCharge += (charge / divisor);
        Debug.Log(arm.name + " Ultiamte Charge: " + arm.UltimateCharge);
    }
}