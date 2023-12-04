using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public abstract class Arm : NetworkBehaviour, INetworkSerializable
{
    // Variables
    public ArmVariables armVariable;        // Ensure ArmVariables is serializable if it contains network-relevant data
    public NetworkVariable<WeaponState> networkWeaponState = new NetworkVariable<WeaponState>();

    // Variables (Combat)
    [SerializeField]
    protected List<GameObject> projectiles; // Consider serializing relevant state information instead of GameObjects
    [SerializeField]
    protected GameObject shootPoint;        // Same as above, consider what needs to be synchronized
    protected GameObject basicProjectile;   
    protected GameObject spellProjectile;   
    protected GameObject ultimateProjectile;
    protected int maxSkillCharges;
    protected int maxSkillInstantiations;
    private float _skillCoolDown;
    private int _skillCharges;
    private float _ultimateCharge;

    protected float nextBasicFireTime;

    // Variables (Audio)
    protected AudioSource audioSource;
    public AudioClip basicAttackSFX;        // Assign this in the Inspector
    public AudioClip skillSFX;              // Assign this in the Inspector
    public AudioClip ultimateSFX;           // Assign this in the Inspector

    // Variables (Visual)
    [SerializeField]
    private Animator animator;
    public float basicAttackCameraShakeIntensity;
    public float basicAttackCameraShakeDuration;
    public float skillCameraShakeIntensity;
    public float skillCameraShakeDuration; 
    public float ultimateCameraShakeIntensity;
    public float ultimateCameraShakeDuration;

    // Properties (Combat)
    public float SkillCoolDown
    {
        get => _skillCoolDown;
        set => _skillCoolDown = value;
    }
    public int SkillCharges
    {
        get => _skillCharges;
        set => _skillCharges = value;
    }
    public float UltimateCharge
    {
        get => _ultimateCharge;
        set => _ultimateCharge = Mathf.Clamp(value, 0, 100);
    }



    public override void OnNetworkSpawn()
    {
        Initialize();
    }

    public virtual void Initialize()
    {
        basicProjectile = projectiles[0] != null ? projectiles[0] : null;
        spellProjectile = projectiles[1] != null ? projectiles[1] : null;
        ultimateProjectile = projectiles[2] != null ? projectiles[2] : null;

        audioSource = GetComponent<AudioSource>();

        maxSkillCharges = armVariable.skillMaxCharges;
        maxSkillInstantiations = armVariable.skillMaxInstants;
        SkillCharges = maxSkillCharges;
        SkillCoolDown = armVariable.skillCoolDown;
        UltimateCharge = armVariable.ultimateCharge;

        nextBasicFireTime = 0f;
    }

    

// SOUND EFFECTS
    public void CastBasicAttackSFX()    //SERVER ONLY
    {
        if (!IsServer) return;
        if (basicAttackSFX != null && audioSource != null)
        {
            foreach (var playerClientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                Vector3 otherPlayerPosition = NetworkManager.Singleton.ConnectedClients[playerClientId].PlayerObject.transform.position;

                CastBasicAttackSFXClientRpc(otherPlayerPosition, new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[] { playerClientId }
                    }
                });
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void CastBasicAttackSFXServerRpc(ServerRpcParams serverRpcParams = default)
    {
        CastBasicAttackSFX();
    }

    [ClientRpc]
    public void CastBasicAttackSFXClientRpc(Vector3 otherPlayerPosition, ClientRpcParams clientRpcParams = default)
    {
        if (basicAttackSFX != null && audioSource != null)
        {
            // Calculate the distance between this player and the other player
            Vector2 relativePosition = otherPlayerPosition - transform.position;

            float maxPanDistance = 5f;
            float panExponent = 2f;     // A quadratic curve for more pronounced panning
            float volumeExponent = 3f;     // A quadratic curve for more pronounced volume

            // Define the maximum distance at which the sound can be heard
            float maxDistance = 100f;

            // Exponential stereo pan based on the horizontal position (left or right)
            float panStereo = -Mathf.Sign(relativePosition.x) * Mathf.Pow(Mathf.Clamp(Mathf.Abs(relativePosition.x) / maxPanDistance, 0f, 1f), panExponent);
            audioSource.panStereo = panStereo;

            // Adjust volume exponentially based on distance
            float distance = Vector2.Distance(transform.position, otherPlayerPosition);

            float volumeRatio = Mathf.Clamp(1 - (distance / maxDistance), 0, 1);
            float volume = Mathf.Pow(volumeRatio, volumeExponent);

            audioSource.PlayOneShot(basicAttackSFX, volume);
        }
    }



// COMBAT
    // The basic attack method
    [ServerRpc(RequireOwnership = false)]
    public virtual void CastBasicAttackServerRpc(ServerRpcParams serverRpcParams = default) { }

    [ClientRpc]
    public virtual void CastBasicAttackClientRpc(ClientRpcParams clientRpcParams = default) 
    { 
        if (!IsOwner) return;
        Logger.Instance.LogInfo($"Cast Basic Attack ClientRpc called by {OwnerClientId}");
    }
    
    [ServerRpc(RequireOwnership = false)]
    public virtual void CastSkillServerRpc(ServerRpcParams serverRpcParams = default) { }

    [ClientRpc]
    public virtual void CastSkillClientRpc(ClientRpcParams clientRpcParams = default) 
    {
        if (!IsOwner) return;
        Logger.Instance.LogInfo($"Cast Skill ClientRpc called by {OwnerClientId}");
    }

    [ServerRpc(RequireOwnership = false)]
    public virtual void CastUltimateServerRpc(ServerRpcParams serverRpcParams = default) { }

    [ClientRpc]
    public virtual void CastUltimateClientRpc(ClientRpcParams clientRpcParams = default) 
    { 
        if (!IsOwner) return;
        Logger.Instance.LogInfo($"Cast Ultimate ClientRpc called by {OwnerClientId}");
    }

    public void ChargeUltimate(float charge, float divisor)
    {
        if (divisor < 1)
        {
            divisor = 1;
        }
        UltimateCharge += (charge / divisor);
        Debug.Log(armVariable.armName + " Ultimate Charge: " + UltimateCharge);
    }

    public GameObject SpawnProjectile<T>(ulong ownerId, GameObject projectilePrefab, GameObject shootPointPrefab) where T : Spawnables   // SERVER ONLY
    {
        // CLIENT-INCLUSIVE
        GameObject projectileClone = Instantiate(projectilePrefab, shootPointPrefab.transform.position, transform.rotation);
        projectileClone.GetComponent<T>().teamId.Value = transform.root.transform.GetComponent<PlayerController>().teamId.Value;
        projectileClone.transform.GetComponent<NetworkObject>().SpawnWithOwnership(ownerId);

        // CLIENT-EXCLUSIVE - Need to sync with ClientRpc
        projectileClone.layer = transform.root.gameObject.layer;
        projectileClone.GetComponent<T>().instantiatingArm = gameObject.GetComponent<Arm>();

        ulong projectileId = projectileClone.GetComponent<NetworkObject>().NetworkObjectId;

        SpawnProjectileClientRpc(projectileId);
        return projectileClone;
    }

    [ClientRpc]
    public virtual void SpawnProjectileClientRpc(ulong projectileId)
    {
        GameObject projectileClone = NetworkManager.Singleton.SpawnManager.SpawnedObjects[projectileId].gameObject;
        projectileClone.GetComponent<Spawnables>().instantiatingArm = gameObject.GetComponent<Arm>();
    }

    public void FireProjectile(GameObject projectileClone)  // SERVER ONLY
    {
        // FIRE PROJECTILE ON SERVER
        // Network Transform included - Synced to all clients
        Rigidbody2D rb = projectileClone.GetComponent<Rigidbody2D>();
        rb.AddForce(shootPoint.transform.up * armVariable.baseForce, ForceMode2D.Impulse);
    }

// ANIMATION
    public void UpdateWeaponAnimator()  // CLIENT
    {
        if (networkWeaponState.Value == WeaponState.Idle)
        {
            animator.SetBool("isBasicAttack", false);
        }
        else if (networkWeaponState.Value == WeaponState.BasicAttack)
        {
            animator.SetBool("isBasicAttack", true);
        }
    }

    public void UpdateWeaponState(WeaponState newState) //SERVER ONLY
    {
        if (!IsServer) return;
        if (networkWeaponState.Value == newState) return;
        networkWeaponState.Value = newState;
    }

    [ServerRpc]
    public void UpdateWeaponStateServerRpc(WeaponState newState)
    {
        UpdateWeaponState(newState);
    }

    public void AnimationReset()
    {
        UpdateWeaponStateServerRpc(WeaponState.Idle);
    }

    public void ShakeCamera()   // SERVER
    {
        NetworkManager.Singleton.ConnectedClients[OwnerClientId].PlayerObject.GetComponent<PlayerController>().ShakeCameraClientRpc(basicAttackCameraShakeIntensity, basicAttackCameraShakeDuration, new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { OwnerClientId }
            }
        });
    }

// MISCELLANEOUS
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        throw new System.NotImplementedException();
    }

    public enum WeaponState
    {
        Idle,
        BasicAttack,
        SkillAttack,
        UltimateAttack
    }
}
