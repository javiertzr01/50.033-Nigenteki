using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BasicShieldTrigger : ShieldTrigger
{
    public override void TriggerEnter2DLogic(Collider2D other)
    {
        // Check if the collision is with a specific object or has specific properties
        if (other.gameObject.TryGetComponent<Projectile>(out Projectile projectile))
        {
            float projectileDamage = projectile.Damage;

            // This is for BasicShield
            BasicShield shieldArm = transform.parent.gameObject.GetComponent<BasicShield>();

            // Damage the shield
            shieldArm.ShieldHealth -= projectileDamage;
        }
    }

}
