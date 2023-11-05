using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeetleSkillTrigger : MonoBehaviour
{
    [SerializeField]
    private float shieldHealth;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collision is with a specific object or has specific properties
        if (other.gameObject.TryGetComponent<Projectile>(out Projectile projectile))
        {
            float projectileDamage = projectile.Damage;

            // Damage the shield
            shieldHealth -= projectileDamage;

            if (shieldHealth <= 0)
            {
                DestroyShield();
            }
        }
    }

    private void DestroyShield()
    {
        Destroy(gameObject);
    }
}