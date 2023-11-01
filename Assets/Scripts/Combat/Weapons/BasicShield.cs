using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicShield : BasicWeapon
{
    public GameObject shieldPrefab;
    public Transform firePoint;
    private int shieldHealth;

    // Track the currently active shield
    private GameObject currentShield;

    void Awake()
    {
        shieldHealth = combatSO.basicShieldHealth;
    }

    void Update()
    {
        // FUTURE: ADD REGENERATIVE FUNCTION IF SHIELD IS NOT TAKEN OUT FOR SET AMOUNT OF SECONDS
    }

    public override void Shoot()
    {
        if (currentShield != null)
        {
            // Destroy the existing shield if it exists
            Destroy(currentShield);
            currentShield = null; // Reset the currentShield reference
        }
        else
        {
            // Instantiate the shield and attach it to the player
            currentShield = Instantiate(shieldPrefab, firePoint.position, firePoint.rotation, transform);
            ShieldController shieldController = currentShield.GetComponent<ShieldController>();

            if (shieldController != null)
            {
                shieldController.SetHealth(shieldHealth);
            }
        }
    }

    // Destroy projectiles when they collide with the shield.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Projectile"))
        {
            Destroy(other.gameObject);
        }
    }
}
