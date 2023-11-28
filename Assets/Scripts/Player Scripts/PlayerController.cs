using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;
using UnityEngine.InputSystem;
using Cinemachine;
using System;

public class PlayerController : NetworkBehaviour
{

    public UnityEvent cameraFollow;
    private PlayerInput playerInput;
    public PlayerVariables playerVariables;
    private float moveSpeed;
    //NetworkVariable<float> _currentHealth;

    private NetworkVariable<float> playerHealth = new NetworkVariable<float>();
    private NetworkVariable<float> playerMaxHealth = new NetworkVariable<float>();
    public NetworkVariable<int> teamId = new NetworkVariable<int>();
    public NetworkVariable<int> redCrystalCount = new NetworkVariable<int>();
    public NetworkVariable<int> blueCrystalCount = new NetworkVariable<int>();
    public NetworkVariable<int> greenCrystalCount = new NetworkVariable<int>();
    public NetworkVariable<int[]> KDStats = new NetworkVariable<int[]>(); // 0: Kills, 1: Deaths

    public UnityEvent<float> playerHealthUpdateEventInvoker;
    public UnityEvent<float> playerMaxHealthUpdateEventInvoker;
    //private HealthBar healthBarUI = new HealthBar();

    private NetworkVariable<PlayerState> networkPlayerState = new NetworkVariable<PlayerState>();
    private NetworkVariable<Vector2> spawnPosition = new NetworkVariable<Vector2>();

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
        playerMaxHealth.Value = playerVariables.maxHealth;
        MoveSpeed = playerVariables.moveSpeed;

        playerHealth.Value = playerMaxHealth.Value;
        spawnPosition.Value = transform.position;

        redCrystalCount.Value = 0;
        blueCrystalCount.Value = 0;
        greenCrystalCount.Value = 0;

        int[] newKD = new int[] { 0, 0 };
        KDStats.Value = newKD;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnArmsServerRpc(ServerRpcParams serverRpcParams = default)
    {
        if (armsInitialized) return;

        Logger.Instance.LogInfo($"Spawning arms on {OwnerClientId}");

        GameObject leftArmHolderClone = Instantiate(leftArmHolderPrefab, player.transform.GetComponent<NetworkObject>().transform.position + leftArmHolderPrefab.transform.localPosition, Quaternion.Euler(0, 0, 0));
        leftArmHolderClone.transform.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
        leftArmHolderClone.GetComponent<NetworkObject>().TrySetParent(player.transform);
        leftArmHolderClone.layer = player.layer;
        leftArmHolder = leftArmHolderClone;

        GameObject rightArmHolderClone = Instantiate(rightArmHolderPrefab, player.transform.GetComponent<NetworkObject>().transform.position + rightArmHolderPrefab.transform.localPosition, Quaternion.Euler(0, 0, 0));
        rightArmHolderClone.transform.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
        rightArmHolderClone.GetComponent<NetworkObject>().TrySetParent(player.transform);
        rightArmHolderClone.layer = player.layer;
        rightArmHolder = rightArmHolderClone;

        GameObject leftArmClone = Instantiate(leftArmPrefab, leftArmHolderClone.transform);
        leftArmClone.transform.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
        leftArmClone.transform.GetComponent<NetworkObject>().TrySetParent(leftArmHolderClone.transform);
        leftArmClone.layer = player.layer;

        GameObject rightArmClone = Instantiate(rightArmPrefab, rightArmHolderClone.transform);
        rightArmClone.transform.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
        rightArmClone.transform.GetComponent<NetworkObject>().TrySetParent(rightArmHolderClone.transform);
        rightArmClone.layer = player.layer;

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
    public void TakeDamageServerRpc(float damage, ulong clientId)
    {
        var damagedClient = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerController>();

        if ((damagedClient.playerHealth.Value - damage) <= 0)
        {
            RespawnServerRpc(clientId);
        }
        else
        {
            damagedClient.playerHealth.Value -= damage;
        }

        Logger.Instance.LogInfo($"Player {clientId} took {damage} damage and has {damagedClient.playerHealth.Value}");
    }

    [ClientRpc]
    public void TakeDamageClientRpc(ClientRpcParams clientRpcParams = default)
    {

    }

    [ServerRpc(RequireOwnership = false)]
    public void SetRespawnServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var setClient = NetworkManager.Singleton.ConnectedClients[OwnerClientId].PlayerObject.GetComponent<PlayerController>();

        setClient.spawnPosition.Value = setClient.transform.position;

        Logger.Instance.LogInfo($"Set Player {OwnerClientId} spawn position as {setClient.spawnPosition.Value}");
    }

    [ServerRpc(RequireOwnership = false)]
    public void RespawnServerRpc(ulong clientId, ServerRpcParams serverRpcParams = default)
    {
        var respawnClient = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerController>();

        respawnClient.transform.position = respawnClient.spawnPosition.Value;

        respawnClient.playerHealth.Value = playerMaxHealth.Value;

        RespawnClientRpc(new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientId }
            }
        });

        Logger.Instance.LogInfo($"Player {clientId} has respawned at {respawnClient.spawnPosition.Value}");
    }

    [ClientRpc]
    public void RespawnClientRpc(ClientRpcParams clientRpcParams = default)
    {
        transform.position = spawnPosition.Value;
    }

    [ServerRpc(RequireOwnership = false)]
    public void CollectCrystalServerRpc(Crystal.CrystalType crysalType, ulong clientId)
    {
        var collectingClient = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerController>();

        if (crysalType == Crystal.CrystalType.Red)
        {
            collectingClient.redCrystalCount.Value += 1;
            foreach (var otherClientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (otherClientId != clientId && NetworkManager.Singleton.ConnectedClients[otherClientId].PlayerObject.transform.GetComponent<PlayerController>().teamId.Value == collectingClient.teamId.Value)
                {
                    NetworkManager.Singleton.ConnectedClients[otherClientId].PlayerObject.transform.GetComponent<PlayerController>().redCrystalCount.Value += 1;
                }
            }
        }
        else if (crysalType == Crystal.CrystalType.Green)
        {
            collectingClient.greenCrystalCount.Value += 1;
            foreach (var otherClientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (otherClientId != clientId && NetworkManager.Singleton.ConnectedClients[otherClientId].PlayerObject.transform.GetComponent<PlayerController>().teamId.Value == collectingClient.teamId.Value)
                {
                    NetworkManager.Singleton.ConnectedClients[otherClientId].PlayerObject.transform.GetComponent<PlayerController>().redCrystalCount.Value += 1;
                }
            }
        }
        else if (crysalType == Crystal.CrystalType.Blue)
        {
            collectingClient.blueCrystalCount.Value += 1;
            foreach (var otherClientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (otherClientId != clientId && NetworkManager.Singleton.ConnectedClients[otherClientId].PlayerObject.transform.GetComponent<PlayerController>().teamId.Value == collectingClient.teamId.Value)
                {
                    NetworkManager.Singleton.ConnectedClients[otherClientId].PlayerObject.transform.GetComponent<PlayerController>().redCrystalCount.Value += 1;
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateTeamCrystalsServerRpc(ulong collectingClientId)
    {
        foreach (var otherClientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (otherClientId != collectingClientId && 
                NetworkManager.Singleton.ConnectedClients[otherClientId].PlayerObject.transform.GetComponent<PlayerController>().teamId.Value == NetworkManager.Singleton.ConnectedClients[collectingClientId].PlayerObject.transform.GetComponent<PlayerController>().teamId.Value)
            {
                NetworkManager.Singleton.ConnectedClients[otherClientId].PlayerObject.transform.GetComponent<PlayerController>().redCrystalCount.Value += 1;
            }
        }
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

    /*public NetworkVariable<float> currentHealth
    {
        get
        {
            return _currentHealth;
        }
        set
        {
            _currentHealth = value;
        }
    }*/

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (!IsOwner && !IsClient) return;
        player = transform.gameObject;
        SpawnArmsServerRpc();
        SetRespawnServerRpc();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            listener.enabled = true;
            vc.Priority = 1;
            playerHealth.OnValueChanged += OnPlayerHealthChanged;
            playerMaxHealth.OnValueChanged += OnPlayerMaxHealthChanged;
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

    public void OnPlayerHealthChanged(float previous, float current)
    {
        playerHealthUpdateEventInvoker.Invoke(current);
    }

    public void OnPlayerMaxHealthChanged(float previous, float current)
    {
        playerMaxHealthUpdateEventInvoker.Invoke(current);
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
        Walking,
        Damaged,
        Death
    }
}
