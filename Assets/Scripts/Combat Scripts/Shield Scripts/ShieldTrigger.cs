using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
public abstract class ShieldTrigger : NetworkBehaviour
{
    [System.NonSerialized]
    public GameObject instantiatingArm; // References the Arm that instantiated this shield
    public NetworkVariable<int> teamId = new NetworkVariable<int>();


    void OnTriggerEnter2D(Collider2D other)
    {
        TriggerEnter2DLogic(other);
    }
    public abstract void TriggerEnter2DLogic(Collider2D other);

    [ServerRpc(RequireOwnership = false)]
    public void DestroyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;

        // if (OwnerClientId != clientId) return;

        transform.GetComponent<NetworkObject>().Despawn(true);
        Destroy(gameObject); // Destroy the projectile

    }

    [ServerRpc(RequireOwnership = false)]
    public virtual void TakeDamageServerRpc(float damage, ulong clientId) { }
}
