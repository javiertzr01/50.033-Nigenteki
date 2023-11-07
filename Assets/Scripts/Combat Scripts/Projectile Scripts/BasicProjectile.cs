using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : Projectile
{
    public override void CollisionEnter2DLogic(Collision2D collision)
    {
        DestroyServerRpc();
    }


    public override void TriggerEnter2DLogic(Collider2D other)
    {
        /*if (other.gameObject.tag == "shield")
        {
            DestroyServerRpc();
        }*/
        DestroyServerRpc();
    }



}
