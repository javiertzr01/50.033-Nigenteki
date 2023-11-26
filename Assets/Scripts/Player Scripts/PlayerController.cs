using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;
using UnityEngine.InputSystem;
using System;

public class PlayerController : NetworkBehaviour
{

    public UnityEvent cameraFollow;
    private PlayerInput playerInput;
    public PlayerVariables playerVariables;
    [System.NonSerialized] public float maxHealth;
    private NetworkVariable<float> moveSpeed = new NetworkVariable<float>();
    // NetworkVariable<float> _currentHealth;
    [System.NonSerialized] public NetworkVariable<float> playerHealth = new NetworkVariable<float>();
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
    private GameObject leftArmPrefab;
    [SerializeField]
    private GameObject rightArmPrefab;

    private GameObject player;
    private GameObject leftArmHolder;
    private GameObject rightArmHolder;
    private GameObject leftArm;
    private GameObject rightArm;
    private bool armsInitialized = false;
    private bool rightArmBasicUse = false;
    private bool leftArmBasicUse = false;
    private NetworkVariable<float> damageTakenScale = new NetworkVariable<float>(); // Reduce/Increase Damage Taken
    [System.NonSerialized] public float passiveHealthRegenerationPercentage = 0f; // Health Regeneration Percentage
    private float secondTicker = 0f;
    [System.NonSerialized] public bool interactingWithHoneyComb = false;
    [System.NonSerialized] public float healingPerSecond = 0f; // different from passiveHealthRegenerationPercentage as it can be interrupted, and is a flat amount
    private float lastDamageTime = -2f; // Initialize to -2 so that healing can start immediately if no damage is taken at the start

    // Implemented for Dash Function
    private bool doForce = false;
    private TrailRenderer tr;
    private Vector2 lastDashVelocity;
    public float dashFactor = 500f; // Arbitrary number chosen that feels right

    // Status
    public NetworkVariable<bool> immuneStun = new NetworkVariable<bool>(false);


    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        maxHealth = playerVariables.maxHealth;
        // _currentHealth = playerVariables.currentHealth;
        playerHealth.Value = playerVariables.maxHealth;
        spawnPosition.Value = transform.position;
        tr = GetComponent<TrailRenderer>();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnArmsServerRpc(ServerRpcParams serverRpcParams = default)
    {
        if (armsInitialized) return;

        Logger.Instance.LogInfo($"Spawning arms on {OwnerClientId}");

        player = NetworkManager.Singleton.ConnectedClients[OwnerClientId].PlayerObject.gameObject;

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
    public void TakeDamageServerRpc(float damage, ulong clientId)
    {
        var damagedClient = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerController>();
        // Update the time when damage is taken
        lastDamageTime = Time.time;

        float calculatedDamage = damage * DamageTakenScale;
        if ((damagedClient.playerHealth.Value - calculatedDamage) <= 0)
        {
            Logger.Instance.LogInfo("Health below 0 - Respawn: " + damagedClient);
            damagedClient.playerHealth.Value = 0;
            // RespawnServerRpc(clientId);
        }
        else
        {
            damagedClient.playerHealth.Value -= calculatedDamage;
            Logger.Instance.LogInfo($"Player {clientId} took {calculatedDamage} damage and has {damagedClient.playerHealth.Value}");

        }

    }

    [ClientRpc]
    public void TakeDamageClientRpc(ClientRpcParams clientRpcParams = default)
    {

    }

    [ServerRpc(RequireOwnership = false)]
    public void HealPlayerServerRpc(float heal, ulong clientId)
    {
        var healedClient = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerController>();

        if ((healedClient.playerHealth.Value + heal) >= maxHealth)
        {
            healedClient.playerHealth.Value = maxHealth;
        }
        else
        {
            healedClient.playerHealth.Value += heal;
            Logger.Instance.LogInfo($"Player {clientId} restored {heal} health and has {healedClient.playerHealth.Value}");
        }
    }

    [ClientRpc]
    public void HealPlayerClientRpc(ClientRpcParams clientRpcParams = default)
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

        respawnClient.playerHealth.Value = maxHealth;

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
    public void UpdatePlayerStateServerRpc(PlayerState newState)
    {
        networkPlayerState.Value = newState;
    }


    public float MoveSpeed
    {
        get => moveSpeed.Value;
        set
        {
            Logger.Instance.LogInfo($"Adjusted MoveSpeed to {value} on {OwnerClientId}");
            moveSpeed.Value = value;
        }
    }

    public float DamageTakenScale
    {
        get => damageTakenScale.Value;
        set
        {
            Logger.Instance.LogInfo($"Adjusted DamageTakenScale to {value} on {OwnerClientId}");
            damageTakenScale.Value = value;
        }
    }

    // public NetworkVariable<float> currentHealth
    // {
    //     get
    //     {
    //         return _currentHealth;
    //     }
    //     set
    //     {
    //         _currentHealth = value;
    //     }
    // }

    // private void DestroyAllChildObjects(GameObject parentGameObject)
    // {
    //     // Check if the parent GameObject has any children
    //     if (parentGameObject.transform.childCount > 0)
    //     {
    //         // Loop through all child objects and destroy them
    //         foreach (Transform child in parentGameObject.transform)
    //         {
    //             Destroy(child.gameObject);
    //         }
    //     }
    // }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (!IsOwner && !IsClient) return;
        SpawnArmsServerRpc();
        GetCameraFollow();

        MoveSpeed = playerVariables.moveSpeed;
        DamageTakenScale = 1f;
    }


    // Update is called once per frame
    void Update()
    {
        if (!IsOwner && !IsClient) return;

        Movement();
        Look();
        LeftArmBasicAttack();
        RightArmBasicAttack();

        bool timeSinceLastDamage = Time.time - lastDamageTime >= 2f;
        if (Time.time >= secondTicker)
        {
            if (passiveHealthRegenerationPercentage > 0f)
            {
                Debug.Log("Passive Regen per second: " + passiveHealthRegenerationPercentage);
                HealPlayerServerRpc(maxHealth * passiveHealthRegenerationPercentage, GetComponent<NetworkObject>().OwnerClientId);
            }

            if (healingPerSecond > 0f && timeSinceLastDamage)
            {
                Debug.Log("Healing per second: " + healingPerSecond);
                HealPlayerServerRpc(healingPerSecond, GetComponent<NetworkObject>().OwnerClientId);
            }
            // Set the next second
            secondTicker += 1f;

        }
    }

    void Movement()
    {
        rb.velocity = moveDir * MoveSpeed;
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
        leftArmBasicUse = value;
    }

    void LeftArmBasicAttack()
    {
        if (!leftArmBasicUse) return;
        //leftArmHolder.transform.GetChild(0).GetComponent<Arm>().CastBasicAttackServerRpc();
        transform.GetChild(0).GetChild(0).GetComponent<Arm>().CastBasicAttackServerRpc();
    }

    public void LeftArmSkillCheck(bool value)
    {
        if (value)
        {
            transform.GetChild(0).GetChild(0).GetComponent<Arm>().CastSkillServerRpc();
        }
    }

    public void LeftArmUltimateCheck(bool value)
    {
        if (value)
        {
            transform.GetChild(0).GetChild(0).GetComponent<Arm>().CastUltimateServerRpc();
        }
    }

    public void RightArmBasicAttackCheck(bool value)
    {
        rightArmBasicUse = value;
    }
    void RightArmBasicAttack()
    {
        if (!rightArmBasicUse) return;

        //rightArmHolder.transform.GetChild(0).GetComponent<Arm>().CastBasicAttackServerRpc();
        transform.GetChild(1).GetChild(0).GetComponent<Arm>().CastBasicAttackServerRpc();
    }

    public void RightArmSkillCheck(bool value)
    {
        if (value)
        {
            transform.GetChild(1).GetChild(0).GetComponent<Arm>().CastSkillServerRpc();
        }
    }

    public void RightArmUltimateCheck(bool value)
    {
        if (value)
        {
            transform.GetChild(1).GetChild(0).GetComponent<Arm>().CastUltimateServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestStunServerRpc(float duration, ServerRpcParams rpcParams = default)
    {
        // Check for server and immunity
        if (!IsServer || immuneStun.Value)
        {
            Debug.Log("Either server or immune to stun");
            return;
        }

        // Server-side stun logic
        ApplyStun(duration);

        // Notify the client to apply client-side effects
        ApplyStunClientRpc(duration, new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new ulong[] { OwnerClientId } } });
    }

    [ClientRpc]
    private void ApplyStunClientRpc(float duration, ClientRpcParams rpcParams = default)
    {
        // Check if this is the correct instance of PlayerController
        if (NetworkManager.Singleton.LocalClientId != OwnerClientId)
        {
            Debug.Log("This is not the PlayerController instance we stun");
            return; // This is not the PlayerController instance we need to stun
        }

        // Assuming playerInput is a component on the same GameObject as PlayerController
        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null)
        {
            Debug.LogError("PlayerInput component not found!");
            return;
        }

        // Apply stun effects here
        playerInput.SwitchCurrentActionMap("Stunned");
        StartCoroutine(ReenableInputAfterStun(duration));
    }


    public void ApplyStun(float duration)
    {
        if (!immuneStun.Value)
        {
            // Disable the player's input actions
            playerInput.SwitchCurrentActionMap("Stunned");
            Debug.Log($"Stunned Player on client {OwnerClientId}");

            // Start a coroutine to re-enable input after a duration
            StartCoroutine(ReenableInputAfterStun(duration));
        }
        else
        {
            Debug.Log($"Player is immune to stun on client {OwnerClientId}");
        }
    }

    private IEnumerator ReenableInputAfterStun(float duration)
    {
        yield return new WaitForSeconds(duration);
        playerInput.SwitchCurrentActionMap("Player"); // Switch back to normal input
        // Here you can also end the stun animation or effect if any
        Debug.Log("Unstunned Player");
    }

    [ClientRpc]
    public void TriggerDashClientRpc(ClientRpcParams clientRpcParams = default)
    {
        if (!IsOwner) return;

        Vector2 dashDirection = transform.up; // Calculate the dash direction
        Dash(dashDirection);
    }


    public void Dash(Vector2 dashDirection)
    {

        if (rb != null)
        {
            // Normalize the dash direction to ensure consistent speed in all directions
            dashDirection.Normalize();

            // Apply a force to the Rigidbody2D in the specified direction

            lastDashVelocity = dashDirection * dashFactor;

            doForce = true;
        }
    }


    private void FixedUpdate()
    {
        if (doForce)
        {
            doForce = false;
            // Debug.Log(lastDashVelocity);
            tr.emitting = true;
            rb.AddForce(lastDashVelocity, ForceMode2D.Impulse);
        }
        else
        {
            tr.emitting = false;
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
