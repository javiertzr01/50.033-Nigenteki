using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
public class BeetleShieldTrigger : ShieldTrigger
{
    Beetle arm;

    [SerializeField]
    private Collider2D shieldCollider;
    [SerializeField]
    private SpriteRenderer shieldSprite;
    private float _shieldCurrentHealth;
    private float shieldRegenTimer;
    protected bool _activated;
    protected bool _destroyed;
    public float ShieldHealth
    {
        get
        {
            return _shieldCurrentHealth;
        }
        set
        {
            _shieldCurrentHealth = value;
        }
    }

    public bool Activated
    {
        get
        {
            return _activated;
        }
        set
        {
            _activated = value;
        }
    }

    public bool Destroyed
    {
        get
        {
            return _destroyed;
        }
        set
        {
            _destroyed = value;
        }
    }

    void Start()
    {
        shieldCollider = gameObject.GetComponent<BoxCollider2D>();
        shieldSprite = gameObject.GetComponentInChildren<SpriteRenderer>();

        shieldRegenTimer = 0f;
        Activated = true;
        Destroyed = false;
        ShieldHealth = instantiatingArm.GetComponent<Arm>().armVariable.shieldMaxHealth;

        ToggleShieldServerRpc();
    }


    // Network variable to keep track of the shield's activation state
    private NetworkVariable<bool> isShieldActive = new NetworkVariable<bool>(false);

    [ServerRpc(RequireOwnership = false)]
    public void ToggleShieldServerRpc(ServerRpcParams serverRpcParams = default)
    {
        // Toggle the shield's activation state
        isShieldActive.Value = !isShieldActive.Value;

        // Call the client RPC to update the shield state on all clients
        ToggleShieldClientRpc(isShieldActive.Value);
    }

    [ClientRpc]
    void ToggleShieldClientRpc(bool isActive, ClientRpcParams clientRpcParams = default)
    {
        // Update the shield's collider and sprite renderer based on the received state
        shieldCollider.enabled = isActive;
        shieldSprite.enabled = isActive;
    }


    void Update()
    {
        // Shield regeneration
        if (!isShieldActive.Value)
        {
            shieldRegenTimer += Time.deltaTime;
            if (shieldRegenTimer >= 3.0f) // Regenerate the shield health after 3 seconds of inactivity
            {
                if (ShieldHealth < instantiatingArm.GetComponent<Arm>().armVariable.shieldMaxHealth)
                {
                    ShieldHealth += 15f * Time.deltaTime; // Regenerate 15 HP per second
                    Logger.Instance.LogInfo("BEETLE SHIELD: Regenerating: " + ShieldHealth);
                    Debug.Log("BEETLE SHIELD: Regenerating: " + ShieldHealth);
                    if (ShieldHealth >= instantiatingArm.GetComponent<Arm>().armVariable.shieldMaxHealth)
                    {
                        ShieldHealth = instantiatingArm.GetComponent<Arm>().armVariable.shieldMaxHealth;
                        Destroyed = false; // Reset destroyed flag if the shield is fully regenerated
                        Logger.Instance.LogInfo("BEETLE SHIELD: restored");
                        Debug.Log("BEETLE SHIELD: restored");
                    }
                }
            }
        }
        else
        {
            shieldRegenTimer = 0f; // Reset the timer if the shield is activated again
        }

        // Break shield if shield HP drops to 0 or past 0
        if (ShieldHealth < 0 && !Destroyed)
        {
            ShieldHealth = 0f;
            Destroyed = true;
            Logger.Instance.LogInfo("BEETLE SHIELD: destroyed");
            Debug.Log("BEETLE SHIELD: destroyed");
            if (isShieldActive.Value)
            {
                shieldCollider.enabled = false;
                shieldSprite.enabled = false;
                isShieldActive.Value = false;
            }
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
        Logger.Instance.LogInfo("BEETLE SHIELD HP: " + ShieldHealth);
        if (ShieldHealth <= 0)
        {
            Destroyed = true;
            // Additional logic for destroyed shield
        }

        // Update clients about the shield's status
        UpdateShieldStatusClientRpc(ShieldHealth, Destroyed, new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientId }
            }
        });
    }

    [ClientRpc]
    private void UpdateShieldStatusClientRpc(float health, bool destroyed, ClientRpcParams clientRpcParams = default)
    {
        ShieldHealth = health;
        Destroyed = destroyed;

        // Update the shield's visual or physical state on clients
        shieldCollider.enabled = !destroyed;
        shieldSprite.enabled = !destroyed;
    }
}
