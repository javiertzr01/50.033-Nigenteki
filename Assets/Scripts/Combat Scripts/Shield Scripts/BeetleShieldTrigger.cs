using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BeetleShieldTrigger : ShieldTrigger
{
    Beetle arm;

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
            shieldArm.ShieldHealth -= projectileDamage;
            arm.ChargeUltimate(projectileDamage, 15);


        }
    }
}
