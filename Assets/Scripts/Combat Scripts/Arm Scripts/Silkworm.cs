using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Silkworm : Arm
{
    [SerializeField] private GameObject ultShootPoint;
    private GameObject spellProjectile;
    private GameObject ultimateProjectile;
    private float nextBasicFireTime = 0f;
    private int skillCharges = 0;
    private int maxSkillCharges;
    private float skillCooldownTimer = 0f;

    public override void Initialize()
    {
        base.Initialize();
        maxSkillCharges = 2;

        // Initialize any Silkworm-specific variables here, if needed.
    }

    public override void CastBasicAttack()
    {
        if (Time.time >= nextBasicFireTime)
        {
            // Instantiate the basic attack projectile and apply force
            GameObject basicProjectile = Instantiate(projectiles[1], ultShootPoint.transform.position, transform.rotation);
            basicProjectile.GetComponent<Projectile>().instantiatingArm = gameObject;
            Rigidbody2D rb = basicProjectile.GetComponent<Rigidbody2D>();
            rb.AddForce(ultShootPoint.transform.up * armVariable.baseForce, ForceMode2D.Impulse);

            // Set the cooldown timer for the next basic attack
            nextBasicFireTime = Time.time + armVariable.baseFireRate;
        }
    }

    public override void CastSkill()
    {
        if (skillCharges > 0 && skillCooldownTimer <= 0f)
        {
            // Instantiate the skill projectile (stun effect)
            GameObject skillProjectile = Instantiate(spellProjectile, ultShootPoint.transform.position, transform.rotation);
            skillProjectile.GetComponent<Projectile>().instantiatingArm = gameObject;

            // Decrease the number of available skill charges and start the cooldown timer
            skillCharges--;
            skillCooldownTimer = armVariable.skillCoolDown;
        }
    }

    public override void CastUltimate()
    {
        // Instantiate the ultimate area effect
        GameObject ultimateArea = Instantiate(ultimateProjectile, ultShootPoint.transform.position, transform.rotation);

        // You'll need to implement the logic for the ultimate's behavior
        // This might involve modifying PlayerController scripts as described

        // Start a timer to destroy the ultimate area after 15 seconds
        StartCoroutine(DestroyUltimateArea(ultimateArea, 15f));
    }

    private IEnumerator DestroyUltimateArea(GameObject area, float duration)
    {
        yield return new WaitForSeconds(duration);

        // Implement logic to apply stun to objects with PlayerController script
        // Modify moveSpeed and attackSpeed as described

        // Destroy the ultimate area after the specified duration
        Destroy(area);
    }
}