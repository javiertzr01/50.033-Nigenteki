using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerController : NetworkBehaviour
{

    public UnityEvent cameraFollow;
    private PlayerInput playerInput;
    public PlayerVariables playerVariables;
    float maxHealth;
    private float moveSpeed;
    NetworkVariable<float> _currentHealth;

    private NetworkVariable<PlayerState> networkPlayerState = new NetworkVariable<PlayerState>();

    private Rigidbody2D rb;
    private Vector2 moveDir;
    private Vector2 mousePos;

    [SerializeField]
    private GameObject leftArmHolderPrefab;

    [SerializeField]
    private GameObject rightArmHolderPrefab;

    [SerializeField]
    public GameObject leftArmPrefab;
    [SerializeField]
    public GameObject rightArmPrefab;

    [SerializeField]
    private CinemachineVirtualCamera vc;

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private AudioListener listener;

    private Animator animator;

    private GameObject player;
    private GameObject leftArmHolder;
    private GameObject rightArmHolder;
    private GameObject leftArm;
    private GameObject rightArm;

    private bool armsInitialized = false;

    private bool rightArmBasicUse = false;
    private bool leftArmBasicUse = false;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        maxHealth = playerVariables.maxHealth;
        MoveSpeed = playerVariables.moveSpeed;
        _currentHealth = playerVariables.currentHealth;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnArmsServerRpc(ServerRpcParams serverRpcParams = default)
    {
        if (armsInitialized) return;

        Logger.Instance.LogInfo($"Spawning arms on {OwnerClientId}");
        
        GameObject leftArmHolderClone = Instantiate(leftArmHolderPrefab, player.transform.GetComponent<NetworkObject>().transform.position + leftArmHolderPrefab.transform.localPosition, Quaternion.Euler(0, 0, 0));
        //leftArmHolderClone.transform.GetComponent<NetworkObject>().Spawn(true);
        leftArmHolderClone.transform.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
        leftArmHolderClone.GetComponent<NetworkObject>().TrySetParent(player.transform);
        leftArmHolder = leftArmHolderClone;

        GameObject rightArmHolderClone = Instantiate(rightArmHolderPrefab, player.transform.GetComponent<NetworkObject>().transform.position + rightArmHolderPrefab.transform.localPosition, Quaternion.Euler(0, 0, 0));
        //rightArmHolderClone.transform.GetComponent<NetworkObject>().Spawn(true);
        rightArmHolderClone.transform.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
        rightArmHolderClone.GetComponent<NetworkObject>().TrySetParent(player.transform);
        rightArmHolder = rightArmHolderClone;

        GameObject leftArmClone = Instantiate(leftArmPrefab, leftArmHolderClone.transform);
        //leftArmClone.transform.GetComponent<NetworkObject>().Spawn(true);
        leftArmClone.transform.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
        leftArmClone.transform.GetComponent<NetworkObject>().TrySetParent(leftArmHolderClone.transform);


        GameObject rightArmClone = Instantiate(rightArmPrefab, rightArmHolderClone.transform);
        //rightArmClone.transform.GetComponent<NetworkObject>().Spawn(true);
        rightArmClone.transform.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
        rightArmClone.transform.GetComponent<NetworkObject>().TrySetParent(rightArmHolderClone.transform);


        armsInitialized = true;
        
        SpawnArmsClientRpc(new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { OwnerClientId }
            }
        });


    }

    [ClientRpc]
    public void SpawnArmsClientRpc(ClientRpcParams clientRpcParams = default)
    {
        Logger.Instance.LogInfo($"Spawned arms on {OwnerClientId}");
    }


    [ServerRpc(RequireOwnership = false)]
    public void UpdatePlayerStateServerRpc(PlayerState newState)
    {
        networkPlayerState.Value = newState;
    }

    public float MoveSpeed
    {
        get
        {
            return moveSpeed;
        }
        set
        {
            moveSpeed = value;
        }
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

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (!IsOwner && !IsClient) return;
        player = transform.gameObject;
        SpawnArmsServerRpc();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            listener.enabled = true;
            vc.Priority = 1;
        }
        else
        {
            vc.Priority = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner && !IsClient) return;

        Movement();
        Look();
        LeftArmBasicAttack();
        RightArmBasicAttack();

        UpdateAnimator();
    }

    void Movement()
    {
        rb.velocity = moveDir * MoveSpeed;

        if (rb.velocity.magnitude > 0f)
        {
            UpdatePlayerStateServerRpc(PlayerState.Walking);
        }
        else
        {
            UpdatePlayerStateServerRpc(PlayerState.Idle);
        }
    }

    void Look()
    {
        Vector2 worldMousePos = cam.ScreenToWorldPoint(mousePos);
        Vector2 lookDir = new Vector2((worldMousePos.x - transform.position.x), (worldMousePos.y - transform.position.y));
        //transform.up = lookDir;
        transform.GetChild(1).transform.up = lookDir;
        transform.GetChild(2).transform.up = lookDir;

        float rotZ = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        if (rotZ < 89 && rotZ > -89)
        {
            transform.GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            transform.GetComponent<SpriteRenderer>().flipX = false;
        }

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
        //cameraController.FollowPlayer(player.transform);
    }

    public void LeftArmBasicAttackCheck(bool value)
    {
        leftArmBasicUse = value;
    }

    void LeftArmBasicAttack()
    {
        if (!leftArmBasicUse) return;

        //transform.GetChild(0).GetChild(0).GetComponent<Arm>().CastBasicAttackServerRpc();
        transform.GetChild(1).GetChild(0).GetComponent<Arm>().CastBasicAttackServerRpc();
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
        rightArmBasicUse = value;
    }
    void RightArmBasicAttack()
    {
        if (!rightArmBasicUse) return;

        transform.GetChild(2).GetChild(0).GetComponent<Arm>().CastBasicAttackServerRpc();
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

    private void UpdateAnimator()
    {
        if (networkPlayerState.Value == PlayerState.Walking)
        {
            animator.SetBool("isMoving", true);
        }
        else if (networkPlayerState.Value == PlayerState.Idle)
        {
            animator.SetBool("isMoving", false);
        }
    }

    public enum PlayerState
    {
        Idle,
        Walking
    }
}
