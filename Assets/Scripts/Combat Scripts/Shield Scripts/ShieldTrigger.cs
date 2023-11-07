using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class ShieldTrigger : MonoBehaviour
{
    [System.NonSerialized]
    public GameObject instantiatingArm; // References the Arm that instantiated this shield

    void OnTriggerEnter2D(Collider2D other)
    {
        TriggerEnter2DLogic(other);
    }
    public abstract void TriggerEnter2DLogic(Collider2D other);
}
