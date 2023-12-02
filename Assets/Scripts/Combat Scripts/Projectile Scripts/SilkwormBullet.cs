using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SilkwormBullet : Projectile
{
    public override void OnEnemyShieldTriggerEnter2D(Collider2D other)
    {
        ShieldTrigger shield = other.GetComponent<ShieldTrigger>();     // TODO: Change this
        if (shield != null && teamId.Value != shield.teamId.Value)
        {
            shield.TakeDamageServerRpc(Damage, shield.GetComponent<NetworkObject>().OwnerClientId);
            DestroyServerRpc();
        }
    }
}
