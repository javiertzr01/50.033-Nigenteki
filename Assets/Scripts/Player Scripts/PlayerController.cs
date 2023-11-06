using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{

    public UnityEvent cameraFollow;
    private PlayerInput playerInput;
    public PlayerVariables playerVariables;
    float maxHealth;
    float moveSpeed;
    NetworkVariable<float> _currentHealth;

    private Rigidbody2D rb;
    private Vector2 moveDir;
    private Vector2 mousePos;

    [SerializeField]
    private GameObject leftArmHolder;

    [SerializeField]
    private GameObject rightArmHolder;

    [SerializeField]
    private GameObject leftArmPrefab;
    [SerializeField]
    private GameObject rightArmPrefab;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        maxHealth = playerVariables.maxHealth;
        moveSpeed = playerVariables.moveSpeed;
        _currentHealth = playerVariables.currentHealth;


        // Instantiate and Initialize Basic Arm as the child to the Arm Holder
        DestroyAllChildObjects(leftArmHolder);
        DestroyAllChildObjects(rightArmHolder);
        Instantiate(leftArmPrefab, leftArmHolder.transform);
        Instantiate(rightArmPrefab, rightArmHolder.transform);

    }

    public NetworkVariable<float> currentHealth
    {
        get
        {
            return _currentHealth;
        }
        set
        {
            _currentHealth = value;
        }
    }

    private void DestroyAllChildObjects(GameObject parentGameObject)
    {
        // Check if the parent GameObject has any children
        if (parentGameObject.transform.childCount > 0)
        {
            // Loop through all child objects and destroy them
            foreach (Transform child in parentGameObject.transform)
            {
                Destroy(child.gameObject);
            }
        }
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
        transform.up = lookDir;

    }

    public void MovementCheck(Vector2 value)
    {
        moveDir = value;
    }

    public void LookCheck(Vector2 value)
    {
        mousePos = value;
    }

    public void GetCameraFollow()
    {
        cameraFollow.Invoke();
    }

    public void LeftArmBasicAttackCheck(bool value)
    {
        if (value)
        {
            leftArmHolder.transform.GetChild(0).GetComponent<Arm>().CastBasicAttack();
        }
    }

    public void LeftArmSkillCheck(bool value)
    {
        if (value)
        {
            leftArmHolder.transform.GetChild(0).GetComponent<Arm>().CastSkill();
        }
    }

    public void LeftArmUltimateCheck(bool value)
    {
        if (value)
        {
            leftArmHolder.transform.GetChild(0).GetComponent<Arm>().CastUltimate();
        }
    }

    public void RightArmBasicAttackCheck(bool value)
    {
        if (value)
        {
            rightArmHolder.transform.GetChild(0).GetComponent<Arm>().CastBasicAttack();
        }
    }

    public void RightArmSkillCheck(bool value)
    {
        if (value)
        {
            rightArmHolder.transform.GetChild(0).GetComponent<Arm>().CastSkill();
        }
    }

    public void RightArmUltimateCheck(bool value)
    {
        if (value)
        {
            rightArmHolder.transform.GetChild(0).GetComponent<Arm>().CastUltimate();
        }
    }

    public void ApplyStun(float duration)
    {
        // Disable the player's input actions
        playerInput.SwitchCurrentActionMap("Stunned");
        Debug.Log("Stunned Player");

        // Start a coroutine to re-enable input after a duration
        StartCoroutine(ReenableInputAfterStun(duration));
    }

    private IEnumerator ReenableInputAfterStun(float duration)
    {
        // Wait for the specified duration
        yield return new WaitForSeconds(duration);

        // Re-enable the default action map
        playerInput.SwitchCurrentActionMap("Player");
        Debug.Log("Unstunned Player");
    }


}
