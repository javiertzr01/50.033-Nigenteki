using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
public abstract class ShieldTrigger : Spawnables
{
    [System.NonSerialized]

    protected bool _destroyed;
    [SerializeField]
    private float _shieldCurrentHealth;
    [SerializeField]
    protected Animator animator;
    
    public float shieldMaxHealth;

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

    public virtual void Start()
    {
        switch (instantiatingArm.armLevel.Value)
        {
            case(ArmLevel.Default):
                ShieldHealth = instantiatingArm.armVariable.shieldMaxHealth;
                break;
            case(ArmLevel.Upgraded):
                ShieldHealth = instantiatingArm.armVariable.shieldMaxHealthUpgraded;
                break;
            case(ArmLevel.Max):
                ShieldHealth = instantiatingArm.armVariable.shieldMaxHealthUpgraded;    // Intentional. Only default and upgraded values
                break;
        }
        shieldMaxHealth = ShieldHealth;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        TriggerEnter2DLogic(other);
    }
    public abstract void TriggerEnter2DLogic(Collider2D other);

    public void DestroyShield()
    {
        transform.GetComponent<NetworkObject>().Despawn(true);
        Destroy(gameObject); // Destroy the projectile
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyShieldServerRpc(ServerRpcParams serverRpcParams = default)
    {
        DestroyShield();
    }

    [ServerRpc(RequireOwnership = false)]
    public virtual void TakeDamageServerRpc(float damage, ulong clientId) { }
}