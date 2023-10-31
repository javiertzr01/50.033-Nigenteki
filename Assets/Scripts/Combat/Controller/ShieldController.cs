using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldController : MonoBehaviour
{
    private int currentHealth;

    void Start()
    {
        // Initialize the shield with the specified health.
        SetHealth(100);
    }

    public void SetHealth(int health)
    {
        currentHealth = health;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            DestroyShield();
        }
    }

    private void DestroyShield()
    {
        Destroy(gameObject);
    }
}
