using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
public abstract class SkillObject : NetworkBehaviour
{
    [System.NonSerialized]
    public Arm instantiatingArm;

    void OnCollisionEnter2D(Collision2D collider)
    {
        CollisionEnter2DLogic(collider);
    }

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
    public abstract void CollisionEnter2DLogic(Collision2D collider);

    [ServerRpc(RequireOwnership = false)]
    public void DestroyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;

        // if (OwnerClientId != clientId) return;

        transform.GetComponent<NetworkObject>().Despawn(true);
        Destroy(gameObject); // Destroy the projectile

    }
}
