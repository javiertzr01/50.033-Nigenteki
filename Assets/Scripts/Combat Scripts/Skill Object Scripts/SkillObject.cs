using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
public abstract class SkillObject : Spawnables
{

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        ColorSpriteToTeamId();
    }

    protected virtual void ColorSpriteToTeamId()
    {
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Color tint = teamId.Value == 0 ? new Color(1f, 0.5f, 0.5f, 1f) : new Color(0.5f, 0.5f, 1f, 1f);  // Light hue of red and blue
        spriteRenderer.color = new Color(tint.r, tint.g, tint.b, spriteRenderer.color.a);
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

    [ServerRpc(RequireOwnership = false)]
    public virtual void DestroyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;

        // if (OwnerClientId != clientId) return;

        GetComponent<NetworkObject>().Despawn(true);
        Destroy(gameObject); // Destroy the projectile
    }
}
