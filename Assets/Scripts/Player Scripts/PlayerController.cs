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

    [SerializeField]
    private GameObject leftArmHolderPrefab;

    [SerializeField]
    private GameObject rightArmHolderPrefab;

    [SerializeField]
    private GameObject baseArmPrefab;

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
        maxHealth = playerVariables.maxHealth;
        moveSpeed = playerVariables.moveSpeed;
        currentHealth = playerVariables.currentHealth;
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

        GameObject leftArmClone = Instantiate(baseArmPrefab, leftArmHolderClone.transform);
        //leftArmClone.transform.GetComponent<NetworkObject>().Spawn(true);
        leftArmClone.transform.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
        leftArmClone.transform.GetComponent<NetworkObject>().TrySetParent(leftArmHolderClone.transform);


        GameObject rightArmClone = Instantiate(baseArmPrefab, rightArmHolderClone.transform);
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

        //rightArmHolder.transform.GetChild(0).GetComponent<Arm>().CastBasicAttackServerRpc();
        transform.GetChild(1).GetChild(0).GetComponent<Arm>().CastBasicAttackServerRpc();
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
}
