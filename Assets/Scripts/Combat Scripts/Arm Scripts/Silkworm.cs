using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Silkworm : Arm
{
    [SerializeField] private GameObject ultShootPoint;
    private GameObject spellProjectile;
    private GameObject ultimateProjectile;
    private float nextBasicFireTime = 0f;
    private int skillCharges;
    private int maxSkillCharges;
    private int maxSkillInstantiations;
    private float skillCooldownTimer = 0f;

    public override void Initialize()
    {
        base.Initialize();
        maxSkillCharges = armVariable.skillMaxCharges;
        maxSkillInstantiations = armVariable.skillMaxInstants;

        skillCharges = maxSkillCharges; // Initialize Arm with the maximum skill Charges first

        StartCoroutine(SkillChargeCooldown());

    }

    public override void CastBasicAttack()
    {
        if (Time.time >= nextBasicFireTime)
        {
            // Instantiate the basic attack projectile and apply force
            GameObject shotBasicProjectile = Instantiate(basicProjectile, shootPoint.transform.position, transform.rotation);
            shotBasicProjectile.GetComponent<Projectile>().instantiatingArm = gameObject;
            Rigidbody2D rb = shotBasicProjectile.GetComponent<Rigidbody2D>();
            rb.AddForce(shootPoint.transform.up * armVariable.baseForce, ForceMode2D.Impulse);
            Debug.Log("Casting " + armVariable.armName + "'s Basic Attack with damage: " + shotBasicProjectile.GetComponent<Projectile>().Damage);


            // Set the cooldown timer for the next basic attack
            nextBasicFireTime = Time.time + armVariable.baseFireRate;
        }
    }

    // This coroutine increments skill charges over time, respecting a maximum limit.
    private IEnumerator SkillChargeCooldown()
    {
        while (true)
        {
            // Wait for the skill cooldown duration
            yield return new WaitForSeconds(armVariable.skillCoolDown);

            // Increment skillCharges if it's not at the maximum limit
            if (skillCharges < maxSkillCharges)
            {
                skillCharges++;
                Debug.Log("Increment Silkworm Skill Charge: " + skillCharges);

            }
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