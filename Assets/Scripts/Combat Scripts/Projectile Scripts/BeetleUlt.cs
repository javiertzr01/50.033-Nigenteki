using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class BeetleUlt : BeetleLaser
{
    public override void OnProjectileTriggerEnter2D(Collider2D other)
    {
        // Destroy Enemy Projectiles
        if (other.transform.GetComponent<Projectile>().teamId.Value != teamId.Value)
        {
            other.transform.GetComponent<Projectile>().DestroyServerRpc();
        }
    }
}
