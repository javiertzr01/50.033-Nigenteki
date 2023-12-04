using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;
using UnityEngine.InputSystem;
using System;
using Cinemachine;

public class PlayerController : NetworkBehaviour
{
    private PlayerInput playerInput;
    public PlayerVariables playerVariables;
    public float defaultMoveSpeed;
    public float defaultDamageTakenScale;
    [SerializeField]
    private NetworkVariable<float> moveSpeed = new NetworkVariable<float>();
    [System.NonSerialized] public NetworkVariable<float> playerHealth = new NetworkVariable<float>();
    public NetworkVariable<float> playerMaxHealth = new NetworkVariable<float>();
    public NetworkVariable<int> teamId = new NetworkVariable<int>();
    public NetworkVariable<int> redCrystalCount = new NetworkVariable<int>();
    public NetworkVariable<int> blueCrystalCount = new NetworkVariable<int>();
    public NetworkVariable<int> greenCrystalCount = new NetworkVariable<int>();
    public NetworkVariable<int> kills = new NetworkVariable<int>();
    public NetworkVariable<int> deaths = new NetworkVariable<int>();
    public NetworkVariable<CharacterSpriteMap> sprite = new NetworkVariable<CharacterSpriteMap>();

    public UnityEvent<float> playerHealthUpdateEventInvoker;
    public UnityEvent<float> playerMaxHealthUpdateEventInvoker;

    private NetworkVariable<PlayerState> networkPlayerState = new NetworkVariable<PlayerState>();
    private NetworkVariable<Vector2> spawnPosition = new NetworkVariable<Vector2>();
    private Rigidbody2D rb;
    private Vector2 moveDir;
    private Vector2 mousePos;

    [SerializeField]
    private GameObject leftArmHolderPrefab;

    [SerializeField]
    private GameObject rightArmHolderPrefab;

    public GameObject leftArmPrefab;
    public GameObject rightArmPrefab;

    [SerializeField]
    public CinemachineVirtualCamera vc;
    private float shakeTimer;

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
    public float dashFactor = 300f; // Arbitrary number chosen that feels right

    // Status
    public NetworkVariable<bool> immuneStun = new NetworkVariable<bool>(false);


    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        playerMaxHealth.Value = playerVariables.maxHealth;
        defaultMoveSpeed = playerVariables.moveSpeed;
        MoveSpeed = playerVariables.moveSpeed;

        playerHealth.Value = playerMaxHealth.Value;
        spawnPosition.Value = transform.position;

        switch(transform.name)
        {
            case("Player Red Defender(Clone)"):
            sprite.Value = CharacterSpriteMap.defender_red;
            break;

            case("Player Red Guardian(Clone)"):
            sprite.Value = CharacterSpriteMap.guardian_red;
            break;

            case("Player Blue Defender(Clone)"):
            sprite.Value = CharacterSpriteMap.defender_blue;
            break;

            case("Player Blue Guardian(Clone)"):
            sprite.Value = CharacterSpriteMap.guardian_blue;
            break;

            default:
            Debug.Log("No character chosen");
            break;
        }

        redCrystalCount.Value = 0;
        blueCrystalCount.Value = 0;
        greenCrystalCount.Value = 0;

        kills.Value = 0;
        deaths.Value = 0;
        defaultDamageTakenScale = 1f;
        DamageTakenScale = 1f;
        tr = GetComponent<TrailRenderer>();
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

        SpawnArmsClientRpc(leftArmHolderClone.GetComponent<NetworkObject>().NetworkObjectId,
                           rightArmHolderClone.GetComponent<NetworkObject>().NetworkObjectId,
                           new ClientRpcParams
                           {
                               Send = new ClientRpcSendParams
                               {
                                   TargetClientIds = new ulong[] { OwnerClientId }
                               }
                           });


    }

    [ClientRpc]
    public void SpawnArmsClientRpc(ulong leftArmHolderId, ulong rightArmHolderId, ClientRpcParams clientRpcParams = default)
    {
        var leftArmHolderObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[leftArmHolderId].gameObject;
        var rightArmHolderObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[rightArmHolderId].gameObject;

        leftArmHolder = leftArmHolderObject;
        rightArmHolder = rightArmHolderObject;
        Logger.Instance.LogInfo($"Spawned arms on {OwnerClientId}");
    }


    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(float damage, ulong attackerClientId, ulong damagedClientId)
    {
        var attackerClient = NetworkManager.Singleton.ConnectedClients[attackerClientId].PlayerObject.GetComponent<PlayerController>();
        var damagedClient = NetworkManager.Singleton.ConnectedClients[damagedClientId].PlayerObject.GetComponent<PlayerController>();
        // Update the time when damage is taken
        lastDamageTime = Time.time;

        float calculatedDamage = damage * DamageTakenScale;
        if ((damagedClient.playerHealth.Value - calculatedDamage) <= 0)
        {
            Logger.Instance.LogInfo("Health below 0 - Respawn: " + damagedClient);
            // damagedClient.playerHealth.Value = 0;
            attackerClient.kills.Value += 1;
            damagedClient.deaths.Value += 1;
            RespawnServerRpc(damagedClientId);
        }
        else
        {
            damagedClient.playerHealth.Value -= calculatedDamage;
            Logger.Instance.LogInfo($"Player {damagedClientId} took {calculatedDamage} damage from {attackerClientId} and has {damagedClient.playerHealth.Value}");

        }

    }

    [ClientRpc]
    public void TakeDamageClientRpc(ClientRpcParams clientRpcParams = default)
    {
    }

    [ServerRpc(RequireOwnership = false)]
    public void IncreaseKillCountServerRpc(ulong clientId)
    {
        var client = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerController>();
        client.kills.Value += 1;
    }

    [ServerRpc(RequireOwnership = false)]
    public void HealPlayerServerRpc(float heal, ulong clientId)
    {
        var healedClient = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerController>();

        if ((healedClient.playerHealth.Value + heal) >= playerMaxHealth.Value)
        {
            healedClient.playerHealth.Value = playerMaxHealth.Value;
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

        respawnClient.playerHealth.Value = playerMaxHealth.Value;

        // Reset Beetle shield health on both arm holders
        ResetBeetleShieldHealthIfPresent(respawnClient.transform.GetChild(1)); // Check left arm holder
        ResetBeetleShieldHealthIfPresent(respawnClient.transform.GetChild(2)); // Check right arm holder


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

    private void ResetBeetleShieldHealthIfPresent(Transform armHolder)
    {
        Beetle beetle = armHolder.GetComponentInChildren<Beetle>();
        if (beetle != null)
        {
            beetle.beetleShieldTrigger.ResetShieldHealth();
        }
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
                    NetworkManager.Singleton.ConnectedClients[otherClientId].PlayerObject.transform.GetComponent<PlayerController>().greenCrystalCount.Value += 1;
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
                    NetworkManager.Singleton.ConnectedClients[otherClientId].PlayerObject.transform.GetComponent<PlayerController>().blueCrystalCount.Value += 1;
                }
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

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (!IsOwner && !IsClient) return;
        player = transform.gameObject;
        SpawnArmsServerRpc();

        MoveSpeed = playerVariables.moveSpeed;
        playerHealth.Value = playerVariables.maxHealth;
        spawnPosition.Value = transform.position;
        playerMaxHealth.Value = playerVariables.maxHealth;
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
        if (!IsOwner || !IsClient) return;
        if (transform.childCount < 2)
        {
            return;
        }
        Movement();
        Look();
        LeftArmBasicAttack();
        RightArmBasicAttack();
        UpdateAnimator();

        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0)
            {
                CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
                    vc.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
            }
        }

        bool timeSinceLastDamage = Time.time - lastDamageTime >= 2f;
        if (Time.time >= secondTicker)
        {
            if (passiveHealthRegenerationPercentage > 0f)
            {
                Debug.Log("Passive Regen per second: " + passiveHealthRegenerationPercentage);
                HealPlayerServerRpc(playerMaxHealth.Value * passiveHealthRegenerationPercentage, GetComponent<NetworkObject>().OwnerClientId);
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
        UpdateBeetleShieldDirection(transform.GetChild(1), lookDir); // For Left Arm Holder
        UpdateBeetleShieldDirection(transform.GetChild(2), lookDir); // For Right Arm Holder

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

    void UpdateBeetleShieldDirection(Transform armHolder, Vector2 lookDir)
    {
        if (armHolder != null)
        {
            Beetle beetle = armHolder.GetComponentInChildren<Beetle>();
            if (beetle != null && beetle.currentShield != null)
            {
                beetle.currentShield.transform.up = lookDir;
            }
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
    public void LeftArmBasicAttackCheck(bool value)
    {
        leftArmBasicUse = value;
    }

    void LeftArmBasicAttack()
    {
        if (!leftArmBasicUse) 
        {
            transform.GetChild(1).GetChild(0).GetComponent<Arm>().AnimationReset();
            return;
        }

        //transform.GetChild(0).GetChild(0).GetComponent<Arm>().CastBasicAttackServerRpc();
        transform.GetChild(1).GetChild(0).GetComponent<Arm>().CastBasicAttackServerRpc();
    }

    public void LeftArmSkillCheck(bool value)
    {
        if (value)
        {
            transform.GetChild(1).GetChild(0).GetComponent<Arm>().CastSkillServerRpc();
        }
    }

    public void LeftArmUltimateCheck(bool value)
    {
        if (value)
        {
            transform.GetChild(1).GetChild(0).GetComponent<Arm>().CastUltimateServerRpc();
        }
    }

    public void RightArmBasicAttackCheck(bool value)
    {
        rightArmBasicUse = value;
    }
    void RightArmBasicAttack()
    {
        if (!rightArmBasicUse)
        {
            transform.GetChild(2).GetChild(0).GetComponent<Arm>().AnimationReset();
            return;
        }
        

        transform.GetChild(2).GetChild(0).GetComponent<Arm>().CastBasicAttackServerRpc();
    }

    public void RightArmSkillCheck(bool value)
    {
        if (value)
        {
            transform.GetChild(2).GetChild(0).GetComponent<Arm>().CastSkillServerRpc();
        }
    }

    public void RightArmUltimateCheck(bool value)
    {
        if (value)
        {
            transform.GetChild(2).GetChild(0).GetComponent<Arm>().CastUltimateServerRpc();
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
        Vector2 mouseWorldPosition = cam.ScreenToWorldPoint(mousePos);
        Vector2 dashDirection = (mouseWorldPosition - (Vector2)transform.position).normalized;

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
            if (tr != null)
            {
                tr.emitting = true;

            }
            rb.AddForce(lastDashVelocity, ForceMode2D.Impulse);
        }
        else
        {
            if (tr != null && tr.emitting)
            {
                tr.emitting = false;
            }
        }
    }

    [ClientRpc]
    public void ShakeCameraClientRpc(float intensity, float time, ClientRpcParams clientRpcParams = default)
    {
        Logger.Instance.LogInfo($"Shaking camera on client {OwnerClientId}");

        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
            vc.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
        shakeTimer = time;
    }

    [ServerRpc (RequireOwnership = false)]
    public void AdjustMovementSpeedServerRpc(float speed)
    {
        MoveSpeed = speed;
    }

    [ServerRpc (RequireOwnership = false)]
    public void AdjustDamageTakenScaleServerRpc(float scale)
    {
        DamageTakenScale = scale;
    }

    [ServerRpc (RequireOwnership = false)]
    public void ToggleImmuneStunServerRpc(bool value)
    {
        immuneStun.Value = value;
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
