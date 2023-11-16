using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowTest : MonoBehaviour
{
    [SerializeField] public Transform bow;
    [SerializeField] public GameObject bowObject;
    [SerializeField] public GameObject arrow;
    [SerializeField] public float arrowSpeed;
    [SerializeField] public Transform shootPoint;

    Vector2 direction;
    // Vector2 mousePos;
    Vector3 mousePos;
    GameObject arrowInstance;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // direction = mousePos - (Vector2)gun.position;

        mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
        direction = (Vector2)mousePos - (Vector2)bow.position;

        FaceMouse();

        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void FaceMouse()
    {
        // if (bowObject.name == "Gauntlet (Left)")
        // {
        //     bow.transform.right = -direction;
        // }
        // else if (bowObject.name == "Gauntlet (Right)")
        // {
        //     bow.transform.right = direction;
        // }

        bow.transform.right = -direction;
    }

    void Shoot()
    {
        arrowInstance = Instantiate(arrow, shootPoint.position, shootPoint.rotation);

        // if (bowObject.name == "Gauntlet (Left)")
        // {
        //     arrowInstance.GetComponent<Rigidbody2D>().AddForce(-arrowInstance.transform.right * arrowSpeed);
        // }
        // else if (gauntletObject.name == "Gauntlet (Right)")
        // {
        //     arrowInstance.GetComponent<Rigidbody2D>().AddForce(arrowInstance.transform.right * arrowSpeed);
        // }

        arrowInstance.GetComponent<Rigidbody2D>().AddForce(-arrowInstance.transform.right * arrowSpeed);

    }
}
