using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;
using System;

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
        isColliding = true;
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

    public virtual void TriggerEnter2DLogic(Collider2D other)
    {
        int targetTeam = other.transform.GetComponent<PlayerController>().teamId.Value;

        if (other.gameObject.tag == "Player")
        {
            if (targetTeam != teamId.Value)
            {
                OnEnemyTriggerEnter2D(other);
            }
            else
            {
                OnTeamTriggerEnter2D(other);
            }
        }
        else if (other.gameObject.tag == "Shield") 
        { 
            OnShieldTriggerEnter2D(other);
        }
        else if (other.gameObject.tag == "Projectile") 
        { 
            OnProjectileTriggerEnter2D(other);
        }
        else if (other.gameObject.tag == "ControlPoint") 
        { 
            OnControlPointTriggerEnter2D(other);
        }
        else
        {
            DestroyServerRpc();
        }
    }

    // Can be overwritten
    public virtual void OnEnemyTriggerEnter2D(Collider2D other)
    {
        // ulong attackerClient = transform.GetComponent<NetworkObject>().OwnerClientId;       // Try OwnerClientId by itself
        ulong sourceClientId = OwnerClientId;
        ulong targetClientId = other.transform.GetComponent<NetworkObject>().OwnerClientId;
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(targetClientId, out var connection))
        {
            connection.PlayerObject.GetComponent<PlayerController>().TakeDamageServerRpc(Damage, sourceClientId, targetClientId);

            // Can be overwritten
            ChargeUltimateValue(Damage, 100);

            DestroyServerRpc();
        }
        else
        {
            throw new InvalidOperationException("Damaged client not found in ConnectedClients.");
        }
    }

    public virtual void OnTeamTriggerEnter2D(Collider2D other){ }
    public virtual void OnShieldTriggerEnter2D(Collider2D other){ }           // Params can be changed here
    public virtual void OnProjectileTriggerEnter2D(Collider2D other){ }       // Params can be changed here
    public virtual void OnControlPointTriggerEnter2D(Collider2D other) { }    // Params can be changed here

    public virtual void TriggerStay2DLogic(Collider2D other) { }
    public virtual void TriggerExit2DLogic(Collider2D other) { }

    public virtual void ChargeUltimateValue(float charge, float divisor)
    {
        instantiatingArm.ChargeUltimate(charge, divisor);
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
