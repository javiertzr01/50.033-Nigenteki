using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BeetleShieldTrigger : ShieldTrigger
{
    Beetle arm;

    void Start()
    {
        arm = instantiatingArm.GetComponent<Beetle>();
    }

    public override void TriggerEnter2DLogic(Collider2D other)
    {
        // Check if the collision is with a specific object or has specific properties
        if (other.gameObject.TryGetComponent<Projectile>(out Projectile projectile))
        {
            float projectileDamage = projectile.Damage;

            // This is for BasicShield
            Beetle shieldArm = transform.parent.gameObject.GetComponent<Beetle>();

            // Damage the shield
            shieldArm.ShieldHealth -= projectileDamage;
            ChargeUltimate(projectileDamage, 15);


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
