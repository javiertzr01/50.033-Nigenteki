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
    float maxHealth;
    private float moveSpeed;
    NetworkVariable<float> _currentHealth;

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

    [System.NonSerialized] public float damageTakenScale = 1f; // TODO: Player Take Damage
    [System.NonSerialized] public float damageDealtScale = 1f; // this is for classes to reference for their projectiles
    [System.NonSerialized] public float passiveHealthRegenerationPercentage = 0f; // TODO: In Update(), health regeneration every 1 second
    private float secondTicker = 0f;
    [System.NonSerialized] public bool interactingWithHoneyComb = false;
    [System.NonSerialized] public float healingPerSecond = 0f; // different from passiveHealthRegenerationPercentage as it can be interrupted, and is a flat amount

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        maxHealth = playerVariables.maxHealth;
        MoveSpeed = playerVariables.moveSpeed;
        _currentHealth = playerVariables.currentHealth;
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
        SpawnArmsServerRpc();
        GetCameraFollow();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner && !IsClient) return;

        Movement();
        Look();
        LeftArmBasicAttack();
        RightArmBasicAttack();

        if (passiveHealthRegenerationPercentage > 0f || healingPerSecond > 0f)
        {
            if (Time.time >= secondTicker)
            {
                // TODO: PASSIVE REGEN FUNCTION
                Debug.Log("Passive Regen per second: " + passiveHealthRegenerationPercentage);
                // TODO: HEALING FUNCTION
                Debug.Log("Healing per second: " + healingPerSecond);

                // Set the next second
                secondTicker = Time.time + 1f;
            }

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
