using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;

public abstract class Projectile : NetworkBehaviour
{
    public ProjectileVariables projectileVariable;
    private float _maxDistance;
    private float _damage;

    [System.NonSerialized]
    public Arm instantiatingArm;
    public Vector2 startingPosition;

    public float MaxDistance
    {
        get
        {
            return _maxDistance;
        }

        set
        {
            _maxDistance = value;
        }
    }

    public float Damage
    {
        get
        {
            return _damage;
        }

        set
        {
            _damage = value;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        CollisionEnter2DLogic(collision);
    }

    public abstract void CollisionEnter2DLogic(Collision2D collision);

    void OnTriggerEnter2D(Collider2D other)
    {
        TriggerEnter2DLogic(other);
    }

    public abstract void TriggerEnter2DLogic(Collider2D other);


    [ServerRpc(RequireOwnership = false)]
    public void DestroyAfterDistanceServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;

        if (OwnerClientId != clientId) return;

        if (Vector2.Distance(startingPosition, transform.position) > MaxDistance)
        {
            transform.GetComponent<NetworkObject>().Despawn(true);
            Destroy(gameObject); // Destroy the projectile
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;

        // if (OwnerClientId != clientId) return;

        transform.GetComponent<NetworkObject>().Despawn(true);
        Destroy(gameObject); // Destroy the projectile

    }

    protected void Start()
    {
        startingPosition = transform.position;
        MaxDistance = projectileVariable.maxDistance;
        Damage = projectileVariable.damage;
    }

    void Update()
    {
        DestroyAfterDistanceServerRpc();
    }
}
