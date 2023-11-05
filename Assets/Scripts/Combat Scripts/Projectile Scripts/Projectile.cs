using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
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

    void OnTriggerEnter2D(Collider2D other)
    {
        TriggerEnter2DLogic(other);
    }

    public abstract void TriggerEnter2DLogic(Collider2D other);

    void Awake()
    {
        startingPosition = transform.position;
        Initialize();

    }

    public virtual void Initialize()
    {
        // Initialize Projectiles with the variables from projectileVariable.
        maxDistance = projectileVariable.maxDistance;
        _damage = projectileVariable.damage;
    }

    void Update()
    {
        if (Vector3.Distance(startingPosition, transform.position) > maxDistance)
        {
            Destroy(gameObject); // Destroy the projectile
        }
    }
}
