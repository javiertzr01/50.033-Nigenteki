using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        isShieldActive.Value = true;
        Destroyed = false;
        ShieldHealth = instantiatingArm.GetComponent<Arm>().armVariable.shieldMaxHealth;

        // Initialize the shield's state based on isShieldActive value
        shieldCollider.enabled = isShieldActive.Value;
        shieldSprite.enabled = isShieldActive.Value;
    }



    // Network variable to keep track of the shield's activation state
    public NetworkVariable<bool> isShieldActive = new NetworkVariable<bool>(false);

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
        // Check if the shield is not active
        if (!isShieldActive.Value)
        {
            shieldRegenTimer += Time.deltaTime;

            // Start regeneration after 3 seconds of inactivity
            if (shieldRegenTimer >= 3.0f)
            {
                RegenerateShield();
            }
        }
        else
        {
            // Reset the timer if the shield is active or not destroyed
            shieldRegenTimer = 0f;
        }
    }

    private void RegenerateShield()
    {
        if (ShieldHealth < instantiatingArm.GetComponent<Arm>().armVariable.shieldMaxHealth)
        {
            ShieldHealth += 50f * Time.deltaTime; // Regenerate HP per second
            if (ShieldHealth >= instantiatingArm.GetComponent<Arm>().armVariable.shieldMaxHealth)
            {
                ShieldHealth = instantiatingArm.GetComponent<Arm>().armVariable.shieldMaxHealth;
                Destroyed = false; // Mark shield as not destroyed
            }

            Logger.Instance.LogInfo("Shield Regen HP: " + ShieldHealth);

            // Do not update the visual state here, keep the shield invisible and non-colliding during regeneration
            UpdateShieldStatusClientRpc(ShieldHealth, Destroyed, new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = NetworkManager.Singleton.ConnectedClientsList.Select(c => c.ClientId).ToArray()
                }
            });
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
        }
        Logger.Instance.LogInfo("BEETLE SHIELD HP: " + ShieldHealth);


        // Update clients about the shield's status, including if it's destroyed
        UpdateShieldStatusClientRpc(ShieldHealth, Destroyed, new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = NetworkManager.Singleton.ConnectedClientsList.Select(c => c.ClientId).ToArray()
            }
        });
    }

    [ClientRpc]
    private void UpdateShieldStatusClientRpc(float health, bool destroyed, ClientRpcParams clientRpcParams = default)
    {
        ShieldHealth = health;
        Destroyed = destroyed;
        isShieldActive.Value = (ShieldHealth > 0) && !destroyed;


        // Update the shield's visual or physical state on clients
        if (isShieldActive.Value && !destroyed)
        {
            // If the shield is not destroyed, you can update its state as needed
            // For example, you might want to change the appearance to indicate damage but not disable it completely
            // Update the shield's visual or physical state on clients
            shieldCollider.enabled = true;
            shieldSprite.enabled = true;
        }
        else
        {

            // If the shield is destroyed, disable collider and sprite
            shieldCollider.enabled = false;
            shieldSprite.enabled = false;
        }
    }
}
