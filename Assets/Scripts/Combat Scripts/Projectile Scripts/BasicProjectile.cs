using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
public class BasicProjectile : Projectile

/* 
    Basic Projectile Functionality:
    When projectile hits a player -> Does Damage to Enemy Player ONLY
    When projectile hits a shield -> Does Damage to Enemy Shield ONLY
*/
{
    public override void OnEnemyPlayerTriggerEnter2D(Collider2D other)
    {
        ulong sourceClientId = OwnerClientId;
        ulong targetClientId = other.transform.GetComponent<NetworkObject>().OwnerClientId;
        other.transform.GetComponent<PlayerController>().TakeDamageServerRpc(Damage, sourceClientId, targetClientId);
        InstantiateDamageNumberServerRpc(targetClientId);
        ChargeUltimateValue(Damage, 2);     // This is a ServerRpc too
        DestroyServerRpc();
    }

    public override void OnEnemyShieldTriggerEnter2D(Collider2D other)         // Params can be changed here
    { 
        ShieldTrigger shield = other.GetComponent<ShieldTrigger>();
        if (shield != null)
        {
            shield.TakeDamageServerRpc(Damage, shield.GetComponent<NetworkObject>().OwnerClientId);
            DestroyServerRpc();
        }
    }   
}