using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using Unity.VisualScripting;

public abstract class Projectile : Spawnables
{
    public ProjectileVariables projectileVariable;
    private float _maxDistance;
    private float _damage;
    public bool isColliding = false;

    public Vector2 startingPosition;

    public GameObject damageNumber;
    public string damageNumberAddressableName = "DamageNumber";

    // Properties
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
        Debug.Log("Colliding with: " + other.name);
        if (other.gameObject.tag == "Player")
        {
            OnPlayerTriggerEnter2D(other);
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
        else if (other.gameObject.tag == "ProximityBehaviour") { }
        else
        {
            DestroyServerRpc();
        }
    }

    // ON TRIGGER ENTER
    // Player OnTriggerEnter2D Logic
    public virtual void OnPlayerTriggerEnter2D(Collider2D other)           // Params can be changed here
    {
        if (other.transform.GetComponent<PlayerController>().teamId.Value != teamId.Value)
        {
            OnEnemyPlayerTriggerEnter2D(other);
        }
        else
        {
            OnTeamPlayerTriggerEnter2D(other);
        }
    }
    public virtual void OnEnemyPlayerTriggerEnter2D(Collider2D other) { }    // Params can be changed here
    public virtual void OnTeamPlayerTriggerEnter2D(Collider2D other) { }     // Params can be changed here



    // Shield OnTriggerEnter2D Logic
    public virtual void OnShieldTriggerEnter2D(Collider2D other)            // Params can be changed here
    {
        if (other.transform.GetComponent<ShieldTrigger>().teamId.Value != teamId.Value)
        {
            OnEnemyShieldTriggerEnter2D(other);
        }
        else
        {
            OnTeamShieldTriggerEnter2D(other);
        }
    }
    public virtual void OnEnemyShieldTriggerEnter2D(Collider2D other) { }     // Params can be changed here
    public virtual void OnTeamShieldTriggerEnter2D(Collider2D other) { }      // Params can be changed here



    // Projectile OnTriggerEnter2D Logic
    public virtual void OnProjectileTriggerEnter2D(Collider2D other)         // Params can be changed here
    {
        if (other.transform.GetComponent<Projectile>().teamId.Value != teamId.Value)
        {
            OnEnemyProjectileTriggerEnter2D(other);
        }
        else
        {
            OnTeamProjectileTriggerEnter2D(other);
        }
    }
    public virtual void OnEnemyProjectileTriggerEnter2D(Collider2D other) { }  // Params can be changed here
    public virtual void OnTeamProjectileTriggerEnter2D(Collider2D other) { }   // Params can be changed here



    // Control Point TriggerEnter2D Logic
    public virtual void OnControlPointTriggerEnter2D(Collider2D other) { }     // Params can be changed here



    // ON TRIGGER STAY
    // Player OnTriggerStay2D Logic
    public virtual void OnPlayerTriggerStay2D(Collider2D other)           // Params can be changed here
    {
        if (other.transform.GetComponent<PlayerController>().teamId.Value != teamId.Value)
        {
            OnEnemyPlayerTriggerStay2D(other);
        }
        else
        {
            OnTeamPlayerTriggerStay2D(other);
        }
    }
    public virtual void OnEnemyPlayerTriggerStay2D(Collider2D other) { }    // Params can be changed here
    public virtual void OnTeamPlayerTriggerStay2D(Collider2D other) { }     // Params can be changed here



    // Shield OnTriggerStay2D Logic
    public virtual void OnShieldTriggerStay2D(Collider2D other)            // Params can be changed here
    {
        if (other.transform.GetComponent<ShieldTrigger>().teamId.Value != teamId.Value)
        {
            OnEnemyShieldTriggerStay2D(other);
        }
        else
        {
            OnTeamShieldTriggerStay2D(other);
        }
    }
    public virtual void OnEnemyShieldTriggerStay2D(Collider2D other) { }     // Params can be changed here
    public virtual void OnTeamShieldTriggerStay2D(Collider2D other) { }      // Params can be changed here



    // Projectile OnTriggeeStay2D Logic
    public virtual void OnProjectileTriggerStay2D(Collider2D other)         // Params can be changed here
    {
        if (other.transform.GetComponent<Projectile>().teamId.Value != teamId.Value)
        {
            OnEnemyProjectileTriggerStay2D(other);
        }
        else
        {
            OnTeamProjectileTriggerStay2D(other);
        }
    }
    public virtual void OnEnemyProjectileTriggerStay2D(Collider2D other) { }  // Params can be changed here
    public virtual void OnTeamProjectileTriggerStay2D(Collider2D other) { }   // Params can be changed here



    // Control Point TriggerStay2D Logic
    public virtual void OnControlPointTriggerStay2D(Collider2D other) { }     // Params can be changed here



    // ON TRIGGER EXIT
    // Player OnTriggerExit2D Logic
    public virtual void OnPlayerTriggerExit2D(Collider2D other)           // Params can be changed here
    {
        if (other.transform.GetComponent<PlayerController>().teamId.Value != teamId.Value)
        {
            OnEnemyPlayerTriggerExit2D(other);
        }
        else
        {
            OnTeamPlayerTriggerExit2D(other);
        }
    }
    public virtual void OnEnemyPlayerTriggerExit2D(Collider2D other) { }    // Params can be changed here
    public virtual void OnTeamPlayerTriggerExit2D(Collider2D other) { }     // Params can be changed here



    // Shield OnTriggerExit2D Logic
    public virtual void OnShieldTriggerExit2D(Collider2D other)            // Params can be changed here
    {
        if (other.transform.GetComponent<ShieldTrigger>().teamId.Value != teamId.Value)
        {
            OnEnemyShieldTriggerExit2D(other);
        }
        else
        {
            OnTeamShieldTriggerExit2D(other);
        }
    }
    public virtual void OnEnemyShieldTriggerExit2D(Collider2D other) { }     // Params can be changed here
    public virtual void OnTeamShieldTriggerExit2D(Collider2D other) { }      // Params can be changed here



    // Projectile OnTriggerExit2D Logic
    public virtual void OnProjectileTriggerExit2D(Collider2D other)         // Params can be changed here
    {
        if (other.transform.GetComponent<Projectile>().teamId.Value != teamId.Value)
        {
            OnEnemyProjectileTriggerExit2D(other);
        }
        else
        {
            OnTeamProjectileTriggerExit2D(other);
        }
    }
    public virtual void OnEnemyProjectileTriggerExit2D(Collider2D other) { }  // Params can be changed here
    public virtual void OnTeamProjectileTriggerExit2D(Collider2D other) { }   // Params can be changed here



    // Control Point TriggerExit2D Logic
    public virtual void OnControlPointTriggerExit2D(Collider2D other) { }     // Params can be changed here



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

    public virtual void ChargeUltimateValue(float charge, float divisor)
    {
        Debug.Log("Projectile Script: " + instantiatingArm.ToString());
        instantiatingArm.ChargeUltimateServerRpc(charge, divisor);
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

    public override void OnNetworkSpawn()
    {
        startingPosition = transform.position;
        MaxDistance = projectileVariable.maxDistance;
        Damage = projectileVariable.damage;
        AssignDamageNumberPrefab(damageNumberAddressableName);
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Color tint = teamId.Value == 0 ? new Color(1f, 0.5f, 0.5f, 1f) : new Color(0.5f, 0.5f, 1f, 1f);  // Light hue of red and blue
        spriteRenderer.color = new Color(tint.r, tint.g, tint.b, spriteRenderer.color.a);
    }

    public void AssignDamageNumberPrefab(string name)   // Call this function to change the DamageNumber prefab with an addressable name (Overwrite it in OnNetworkSpawn after base.OnNetworkSpawn())
    {
        AsyncOperationHandle<GameObject> opHandle = Addressables.LoadAssetAsync<GameObject>(name);
        opHandle.WaitForCompletion();

        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            damageNumber = opHandle.Result;
        }
        else
        {
            Debug.Log("Loading prefab failed");
        }
    }

    protected virtual void Update()
    {
        DestroyAfterDistanceServerRpc();
    }
}
