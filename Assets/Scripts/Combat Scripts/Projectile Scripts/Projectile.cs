using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;

public abstract class Projectile : NetworkBehaviour
{
    public ProjectileVariables projectileVariable;
    private float _maxDistance;
    private float _damage;

    protected Vector3 startingPosition;

    public float maxDistance
    {
        get
        {
            return _maxDistance;
        }

        set
        {
            _maxDistance = value;
        }
    }

    public float Damage
    {
        get
        {
            return _damage;
        }

        set
        {
            _damage = value;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        CollisionEnter2DLogic(collision);
    }

    public abstract void CollisionEnter2DLogic(Collision2D collision);

    void Awake()
    {
        startingPosition = transform.position;
    }

    void Update()
    {
        if (Vector3.Distance(startingPosition, transform.position) > maxDistance)
        {
            Destroy(gameObject); // Destroy the projectile
        }
    }
}
