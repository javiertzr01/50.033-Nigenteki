using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : Projectile
{
    public override void CollisionEnter2DLogic(Collision2D collision)
    {
        Destroy(gameObject);
    }

}
