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
    private float skillChargeTimer;
    private int maxSkillInstantiations;

    public override void Initialize()
    {
        base.Initialize();
        maxSkillCharges = armVariable.skillMaxCharges;
        skillChargeTimer = armVariable.skillCoolDown;
        maxSkillInstantiations = armVariable.skillMaxInstants;

        skillCharges = maxSkillCharges; // Initialize Arm with the maximum skill Charges first

        // StartCoroutine(SkillChargeCooldown());

        if (projectiles[1] != null)
        {
            spellProjectile = projectiles[1];
        }

        if (projectiles[2] != null)
        {
            ultimateProjectile = projectiles[2];
        }

    }

    private void Update()
    {
        // Update the skill charge timer
        if (skillCharges < maxSkillCharges)
        {
            skillChargeTimer -= Time.deltaTime;
            if (skillChargeTimer <= 0)
            {
                // Increment skillCharges when the timer reaches 0
                skillCharges++;
                Debug.Log("Increment Silkworm Skill Charge: " + skillCharges);
                // Reset the skill charge timer
                skillChargeTimer = armVariable.skillCoolDown;
            }
        }
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

    // Declare a Queue to track active spellProjectiles
    private Queue<GameObject> activeSpellProjectiles = new Queue<GameObject>();

    public override void CastSkill()
    {
        if (skillCharges > 0)
        {
            // Check if the number of skill instantiations exceeds the maximum
            if (activeSpellProjectiles.Count >= maxSkillInstantiations)
            {
                // Remove the oldest skillProjectile
                GameObject oldestProjectile = activeSpellProjectiles.Dequeue();
                Destroy(oldestProjectile);
                Debug.Log("Removed oldest Silkworm skillProjectile");
            }

            // Instantiate the skill projectile and add it to the active projectiles queue
            GameObject skillProjectile = Instantiate(spellProjectile, shootPoint.transform.position, transform.rotation);
            activeSpellProjectiles.Enqueue(skillProjectile);

            // Decrease the number of available skill charges
            skillCharges--;
            Debug.Log("Decrease Silkworm Skill Charge: " + skillCharges);
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