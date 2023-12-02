using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SilkwormBullet : Projectile
{
    public override void OnShieldTriggerEnter2D(Collider2D other)
    {
        // TODO: Ignore Ally shields
        base.OnShieldTriggerEnter2D(other);
    }
}
