using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class BeetleLaser : Projectile
{
    // Laser should only hit the Players that it pierces through once.
    private HashSet<GameObject> damagedObjects = new HashSet<GameObject>();

    // Override the Start method to call the base class's Start first
    new void Start()
    {
        // Call the base class's Start method
        base.Start();
    }

    public override void OnEnemyTriggerEnter2D(Collider2D other)
    {
        if (!damagedObjects.Contains(other.gameObject))
            base.OnEnemyTriggerEnter2D(other);
            damagedObjects.Add(other.gameObject);
    }

    public override void OnShieldTriggerEnter2D(Collider2D other)
    {
        // TODO: Ignore Ally shields
        base.OnShieldTriggerEnter2D(other);
    }
}
