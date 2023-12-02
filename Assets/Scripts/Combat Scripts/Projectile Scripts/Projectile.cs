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
    public NetworkVariable<int> teamId = new NetworkVariable<int>();
    public bool isColliding = false;

    [System.NonSerialized]
    public Arm instantiatingArm;
    public Vector2 startingPosition;

    public GameObject damageNumber;

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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isColliding) return;
        TriggerEnter2DLogic(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        TriggerStay2DLogic(other);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        TriggerExit2DLogic(other);
    }

    public abstract void TriggerEnter2DLogic(Collider2D other);
    public virtual void TriggerStay2DLogic(Collider2D other) { }
    public virtual void TriggerExit2DLogic(Collider2D other) { }

    [ServerRpc(RequireOwnership = false)]
    public void InstantiateDamageNumberServerRpc(ulong hitClientId, ServerRpcParams serverRpcParams = default)
    {
        if (!IsServer) return;

        Logger.Instance.LogInfo($"Spawning damage number on {hitClientId}");

        GameObject damageNumberClone = Instantiate(damageNumber, NetworkManager.Singleton.ConnectedClients[hitClientId].PlayerObject.transform.position, NetworkManager.Singleton.ConnectedClients[hitClientId].PlayerObject.transform.rotation);
        damageNumberClone.GetComponent<DamageNumber>().Initialize();
        damageNumberClone.GetComponent<NetworkObject>().Spawn();
        damageNumberClone.GetComponent<DamageNumber>().damage.Value = Damage;
    }

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

    protected virtual void Start()
    {
        startingPosition = transform.position;
        MaxDistance = projectileVariable.maxDistance;
        Damage = projectileVariable.damage;
    }

    protected virtual void Update()
    {
        DestroyAfterDistanceServerRpc();
    }
}
