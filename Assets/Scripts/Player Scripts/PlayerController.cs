using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{

    public UnityEvent cameraFollow;

    public PlayerVariables playerVariables;
    float maxHealth;
    float moveSpeed;
    NetworkVariable<float> currentHealth;

    private Rigidbody2D rb;
    private Vector2 moveDir;
    private Vector2 mousePos;
    public BasicWeapon rightArm;
    public BasicWeapon leftArm;

    private void Awake()
    {
        maxHealth = playerVariables.maxHealth;
        moveSpeed = playerVariables.moveSpeed;

        currentHealth = playerVariables.currentHealth;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (!IsOwner && !IsClient) return;
        GetCameraFollow();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner && !IsClient) return;

        Movement();
        Look();

    }

    void Movement()
    {
        rb.velocity = moveDir * moveSpeed;
    }

    void Look()
    {
        Vector2 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector2 lookDir = new Vector2((worldMousePos.x - transform.position.x), (worldMousePos.y - transform.position.y));

        // Rotation of character towards mouse
        // 0 Degrees = Facing Right. Since Player Sprite by Default faces up, we reduce the Euler angle by 90 degrees
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        transform.eulerAngles = new Vector3(0, 0, angle);
        // Debug.Log(angle);

    }

    public void MovementCheck(Vector2 value)
    {
        moveDir = value;
    }

    public void LookCheck(Vector2 value)
    {
        mousePos = value;
    }


    public void LeftArmCheck(bool value)
    {
        if (value)
        {
            Debug.Log("Left Arm Trigger");
            leftArm.Shoot();
        }
    }


    public void RightArmCheck(bool value)
    {
        if (value)
        {
            Debug.Log("Right Arm Trigger");
            rightArm.Shoot();
        }
    }


    public void GetCameraFollow()
    {
        cameraFollow.Invoke();
    }

}
