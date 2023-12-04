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