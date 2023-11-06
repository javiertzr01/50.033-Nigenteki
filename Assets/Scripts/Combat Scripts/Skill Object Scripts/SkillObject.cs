using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class SkillObject : MonoBehaviour
{
    [System.NonSerialized]
    public GameObject instantiatingArm;

    void OnCollisionEnter2D(Collision2D collider)
    {
        CollisionEnter2DLogic(collider);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        TriggerEnter2DLogic(other);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        TriggerExit2DLogic(other);
    }
    public abstract void TriggerEnter2DLogic(Collider2D other);
    public abstract void TriggerExit2DLogic(Collider2D other);
    public abstract void CollisionEnter2DLogic(Collision2D collider);
}
