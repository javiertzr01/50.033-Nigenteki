using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
public abstract class SkillObject : NetworkBehaviour
{
    [System.NonSerialized]
    public Arm instantiatingArm;
    public NetworkVariable<int> teamId = new NetworkVariable<int>();




    void OnTriggerEnter2D(Collider2D other)
    {
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
    public virtual void DestroyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;

        // if (OwnerClientId != clientId) return;

        GetComponent<NetworkObject>().Despawn(true);
        Destroy(gameObject); // Destroy the projectile
    }
}
