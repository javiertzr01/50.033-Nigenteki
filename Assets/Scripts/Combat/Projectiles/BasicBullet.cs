using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Commented out are possible hit effect animations upon collision
public class BasicBullet : MonoBehaviour
{
    // public GameObject hitEffect;
    void OnCollisionEnter2D(Collision2D collision)
    {
        // GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
        // Destroy(effect, 5f);
        Destroy(gameObject);
    }
}
