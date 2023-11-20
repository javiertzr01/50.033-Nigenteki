using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeetleSkillTrigger : ShieldTrigger
{
    Beetle arm;

    [SerializeField]
    private float shieldHealth;
    private float countdownTimer; // Countdown timer

    void Start()
    {
        arm = instantiatingArm.GetComponent<Beetle>();
        countdownTimer = arm.armVariable.skillDuration;
    }

    private void Update()
    {
        if (countdownTimer > 0f)
        {
            countdownTimer -= Time.deltaTime;
            if (countdownTimer <= 0f)
            {
                DestroyServerRpc();
            }
        }
    }

    public override void TriggerEnter2DLogic(Collider2D other)
    {
        // Check if the collision is with a specific object or has specific properties
        if (other.gameObject.TryGetComponent<Projectile>(out Projectile projectile))
        {
            float projectileDamage = projectile.Damage;

            // Damage the shield
            shieldHealth -= projectileDamage;
            arm.ChargeUltimate(projectileDamage, 15);

            if (shieldHealth <= 0)
            {
                DestroyServerRpc();
            }
        }
    }

}