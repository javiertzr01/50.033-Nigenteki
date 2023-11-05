using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BeetleShieldTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collision is with a specific object or has specific properties
        if (other.gameObject.TryGetComponent<Projectile>(out Projectile projectile))
        {
            float projectileDamage = projectile.Damage;

            // This is for BasicShield
            Beetle shieldArm = transform.parent.gameObject.GetComponent<Beetle>();

            // Damage the shield
            shieldArm.ShieldHealth -= projectileDamage;
        }


    }
}
