using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
public class BeetleShieldTrigger : ShieldTrigger
{
    Beetle arm;

    // Network variable to keep track of the shield's activation state
    public NetworkVariable<bool> isShieldActive = new NetworkVariable<bool>(false);
    public NetworkVariable<bool> isAttackingNoShield = new NetworkVariable<bool>(false); 
    [SerializeField]
    private Collider2D shieldCollider;
    [SerializeField]
    private SpriteRenderer shieldSprite;
    private float shieldRegenTimer;
    private ArmLevel currentArmLevel;

    void Awake()
    {
        shieldCollider = gameObject.GetComponent<BoxCollider2D>();
        shieldSprite = gameObject.GetComponentInChildren<SpriteRenderer>();
    }

    public override void Start()
    {
        base.Start();
        shieldRegenTimer = 0f;
        Destroyed = false;
        arm = transform.GetComponentInParent<Beetle>();
        currentArmLevel = arm.armLevel.Value;

        // Initialize the shield's state based on isShieldActive value
        shieldCollider.enabled = isShieldActive.Value;
        shieldSprite.enabled = isShieldActive.Value;
    }

    void Update()
    {
        // Check if the shield is not active
        if (!isShieldActive.Value)
        {
            if (isAttackingNoShield.Value) 
            {
                shieldRegenTimer = 0;
                ToggleAttackingNoShieldServerRpc(false);
            }
            shieldRegenTimer += Time.deltaTime;

            // Start regeneration after 3 seconds of inactivity
            if (shieldRegenTimer >= 3.0f)
            {
                RegenerateShieldServerRpc();
            }
        }
        else
        {
            // Reset the timer if the shield is active or not destroyed
            shieldRegenTimer = 0f;
        }
        if ((shieldCollider.enabled == isShieldActive.Value) && (shieldSprite.enabled == isShieldActive.Value)) {return;}
        else
        {
            shieldCollider.enabled = isShieldActive.Value;
            shieldSprite.enabled = isShieldActive.Value;
        }
    }

    public void ToggleShield()      // SERVER ONLY
    {
        // Toggle the shield's activation state
        isShieldActive.Value = !isShieldActive.Value;
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void ToggleAttackingNoShieldServerRpc(bool value)
    {
        isAttackingNoShield.Value = value;
    }

    [ServerRpc(RequireOwnership = false)]
    public void RegenerateShieldServerRpc()
    {
        if (ShieldHealth < shieldMaxHealth)
        {
            ShieldHealth += 50f * Time.deltaTime; // Regenerate HP per second
            if (ShieldHealth >= shieldMaxHealth)
            {
                ShieldHealth = shieldMaxHealth;   
            }

            if (ShieldHealth >= 0.5 * shieldMaxHealth)
            {
                Destroyed = false; // Mark shield as not destroyed
            }
            // Do not update the visual state here, keep the shield invisible and non-colliding during regeneration
            UpdateShieldStatusClientRpc(ShieldHealth, Destroyed);
        }
    }


    public override void TriggerEnter2DLogic(Collider2D other)
    {
    }

    [ServerRpc(RequireOwnership = false)]
    public override void TakeDamageServerRpc(float damage, ulong clientId)
    {
        if (OwnerClientId != clientId) return;

        ShieldHealth -= damage;
        if (ShieldHealth <= 0f)
        {
            ShieldHealth = 0f;
            Destroyed = true;
            isShieldActive.Value = false;
        }
        Logger.Instance.LogInfo("BEETLE SHIELD HP: " + ShieldHealth);


        // Update clients about the shield's status, including if it's destroyed
        UpdateShieldStatusClientRpc(ShieldHealth, Destroyed);
    }

    [ClientRpc]
    private void UpdateShieldStatusClientRpc(float health, bool destroyed)
    {
        ShieldHealth = health;
        Destroyed = destroyed;
    }

    public void ResetShieldHealth()
    {
        ShieldHealth = shieldMaxHealth;
        Destroyed = false;
        isShieldActive.Value = false; // Optionally reset the active state
        Logger.Instance.LogInfo("Reset Beetle Shield Health to: " + ShieldHealth);
        UpdateShieldStatusClientRpc(ShieldHealth, Destroyed);
    }
}