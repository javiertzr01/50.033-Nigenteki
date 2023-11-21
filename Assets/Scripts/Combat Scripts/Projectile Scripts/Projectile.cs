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
    public GameObject instantiatingArm; // References the Arm that instantiated this projectile

    public Vector2 startingPosition;

    public float maxDistance
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

        if (Vector2.Distance(startingPosition, transform.position) > maxDistance)
        {
            transform.GetComponent<NetworkObject>().Despawn(true);
            Destroy(gameObject); // Destroy the projectile
        }   
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;

        if (OwnerClientId != clientId) return;

        transform.GetComponent<NetworkObject>().Despawn(true);
        Destroy(gameObject); // Destroy the projectile

    }

    void Start()
    {
        startingPosition = transform.position;
    }

    void Update()
    {
        DestroyAfterDistanceServerRpc();
    }
}
