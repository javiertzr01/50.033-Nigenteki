using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Silkworm : BasicWeapon
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    private float bulletForce;

    void Awake()
    {
        bulletForce = combatSO.silkwormBulletSpeed;
    }

    void Update()
    {

    }

    public override void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);
    }
}
