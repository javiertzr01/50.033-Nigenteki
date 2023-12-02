using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class BeetleUlt : BeetleLaser
{
    public override void OnEnemyProjectileTriggerEnter2D(Collider2D other)
    {
        other.transform.GetComponent<Projectile>().DestroyServerRpc();
    }
}
