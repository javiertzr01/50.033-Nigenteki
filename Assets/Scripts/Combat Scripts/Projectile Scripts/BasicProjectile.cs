using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BasicProjectile : Projectile
{
    public override void OnEnemyShieldTriggerEnter2D(Collider2D other)      // TODO: Change this
    {
        base.OnEnemyShieldTriggerEnter2D(other);
        ShieldTrigger shield = other.GetComponent<ShieldTrigger>();
        if (shield != null && teamId.Value != shield.teamId.Value)
        {
            shield.TakeDamageServerRpc(Damage, shield.GetComponent<NetworkObject>().OwnerClientId);
            DestroyServerRpc();
        }
    }
}