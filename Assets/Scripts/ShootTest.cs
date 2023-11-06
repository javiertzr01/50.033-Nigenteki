using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootTest : MonoBehaviour
{
    [SerializeField] public Transform gun;
    [SerializeField] public GameObject bullet;
    [SerializeField] public float bulletSpeed;
    [SerializeField] public Transform shootPoint;

    Vector2 direction;
    Vector2 mousePos;
    GameObject bulletInstance;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        direction = mousePos - (Vector2)gun.position;
        FaceMouse();

        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void FaceMouse()
    {
        gun.transform.right = direction;
    }

    void Shoot()
    {
        bulletInstance = Instantiate(bullet, shootPoint.position, shootPoint.rotation);
        bulletInstance.GetComponent<Rigidbody2D>().AddForce(bulletInstance.transform.right * bulletSpeed);
    }
}
