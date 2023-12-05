using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Mathematics;

public abstract class Arm : NetworkBehaviour, INetworkSerializable
{
    // Variables
    public ArmVariables armVariable;        // Ensure ArmVariables is serializable if it contains network-relevant data
    public NetworkVariable<WeaponState> networkWeaponState = new NetworkVariable<WeaponState>();
    public NetworkVariable<ArmLevel> armLevel = new NetworkVariable<ArmLevel>(ArmLevel.Default);
    public ArmType armType;

    // Variables (Combat)
    [SerializeField]
    protected List<GameObject> projectiles; // Consider serializing relevant state information instead of GameObjects
    [SerializeField]
    protected GameObject shootPoint;        // Same as above, consider what needs to be synchronized
    protected GameObject basicProjectile;
    protected GameObject spellProjectile;
    protected GameObject ultimateProjectile;
    protected GameObject altProjectile;
    [SerializeField]
    protected int maxSkillCharges;
    protected int maxSkillInstantiations;
    protected float countdownTimer;         // Used as Ult Timer
    private float _skillCoolDown;
    [SerializeField]
    private int _skillCharges;
    [SerializeField]
    private float _ultimateCharge;

    protected float nextBasicFireTime;

    // Variables (Audio)
    protected AudioSource audioSource0;
    protected AudioSource audioSource1;
    protected AudioSource audioSource2;
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
        SetProjectiles();

        audioSource0 = gameObject.AddComponent<AudioSource>();
        audioSource1 = gameObject.AddComponent<AudioSource>();
        audioSource2 = gameObject.AddComponent<AudioSource>();

        maxSkillCharges = armVariable.skillMaxCharges;
        maxSkillInstantiations = armVariable.skillMaxInstants;
        SkillCharges = maxSkillCharges;
        SkillCoolDown = armVariable.skillCoolDown;
        UltimateCharge = armVariable.ultimateCharge;
        countdownTimer = armVariable.ultimateDuration;

        nextBasicFireTime = 0f;
    }



    // SOUND EFFECTS
    // Server-side method to loop and play audio
    protected void PlayAudioForAllClients(int attackTypeIndex, ClientRpcParams clientRpcParams = default)
    {
        AudioSource audioSource;
        AudioClip audioClip;
        SetAudioSourceClips(attackTypeIndex, out audioSource, out audioClip);

        if (audioClip != null)
        {
            foreach (var playerClientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                Vector3 otherPlayerPosition = NetworkManager.Singleton.ConnectedClients[playerClientId].PlayerObject.transform.position;

                PlayAudioClientRpc(attackTypeIndex, otherPlayerPosition, new ClientRpcParams
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
    public void CastAttackSFXServerRpc(int attackTypeIndex, ServerRpcParams serverRpcParams = default)
    {
        PlayAudioForAllClients(attackTypeIndex);
    }

    // Method to play audio with given AudioSource and AudioClip
    protected void PlayAudio(int attackTypeIndex, Vector3 otherPlayerPosition)
    {
        AudioSource audioSource;
        AudioClip audioClip;
        SetAudioSourceClips(attackTypeIndex, out audioSource, out audioClip);

        if (audioClip != null && audioSource != null)
        {
            // Calculate the distance between this player and the other player
            Vector2 relativePosition = otherPlayerPosition - transform.position;

            float maxPanDistance = 5f;
            float panExponent = 2f;     // A quadratic curve for more pronounced panning
            float volumeExponent = 3f;  // A quadratic curve for more pronounced volume

            // Define the maximum distance at which the sound can be heard
            float maxDistance = 100f;

            // Exponential stereo pan based on the horizontal position (left or right)
            float panStereo = -Mathf.Sign(relativePosition.x) * Mathf.Pow(Mathf.Clamp(Mathf.Abs(relativePosition.x) / maxPanDistance, 0f, 1f), panExponent);
            audioSource.panStereo = panStereo;

            // Adjust volume exponentially based on distance
            float distance = Vector2.Distance(transform.position, otherPlayerPosition);

            float volumeRatio = Mathf.Clamp(1 - (distance / maxDistance), 0, 1);
            float volume = Mathf.Pow(volumeRatio, volumeExponent);

            audioSource.PlayOneShot(audioClip, volume);
        }
    }

    protected virtual void SetAudioSourceClips(int attackTypeIndex, out AudioSource audioSource, out AudioClip audioClip)
    {
        switch (attackTypeIndex)
        {
            case 2:
                audioSource = audioSource2;
                audioClip = ultimateSFX;
                break;
            case 1:
                audioSource = audioSource1;
                audioClip = skillSFX;
                break;
            default:
            case 0:
                audioSource = audioSource0;
                audioClip = basicAttackSFX;
                break;
        }
    }

    [ClientRpc]
    public void PlayAudioClientRpc(int attackTypeIndex, Vector3 otherPlayerPosition, ClientRpcParams clientRpcParams = default)
    {
        PlayAudio(attackTypeIndex, otherPlayerPosition);
    }

    // COMBAT
    public abstract void SetProjectiles();

    // The basic attack method
    [ServerRpc(RequireOwnership = false)]
    public virtual void CastBasicAttackServerRpc(ServerRpcParams serverRpcParams = default) { }

    [ClientRpc]
    public virtual void CastBasicAttackClientRpc(ClientRpcParams clientRpcParams = default)
    {
        if (!IsOwner) return;
        //Logger.Instance.LogInfo($"Cast Basic Attack ClientRpc called by {OwnerClientId}");
    }

    [ServerRpc(RequireOwnership = false)]
    public virtual void CastSkillServerRpc(ServerRpcParams serverRpcParams = default) { }

    [ClientRpc]
    public virtual void CastSkillClientRpc(ClientRpcParams clientRpcParams = default)
    {
        if (!IsOwner) return;
        //Logger.Instance.LogInfo($"Cast Skill ClientRpc called by {OwnerClientId}");
    }

    [ServerRpc(RequireOwnership = false)]
    public virtual void CastUltimateServerRpc(ServerRpcParams serverRpcParams = default) { }

    [ClientRpc]
    public virtual void CastUltimateClientRpc(ClientRpcParams clientRpcParams = default)
    {
        if (!IsOwner) return;
        //Logger.Instance.LogInfo($"Cast Ultimate ClientRpc called by {OwnerClientId}");
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChargeUltimateServerRpc(float charge, float divisor)
    {
        if (divisor < 1)
        {
            divisor = 1;
        }
        AdjustUltimateCharge(UltimateCharge + (charge / divisor));
        Debug.Log(armVariable.armName + " Ultimate Charge: " + UltimateCharge);
    }

    public void ResetUltimateCharge()
    {
        AdjustUltimateCharge(0f);   // Reset Ultimate Charge
        AdjustUltimateChargeClientRpc(0f);
    }

    public void AdjustUltimateCharge(float charge)  // SERVER ONLY
    {
        UltimateCharge = charge;
        AdjustUltimateChargeClientRpc(charge);
    }

    [ServerRpc(RequireOwnership = false)]
    public void AdjustUltimateChargeServerRpc(float charge)
    {
        AdjustUltimateCharge(charge);
    }

    [ClientRpc]
    public void AdjustUltimateChargeClientRpc(float charge)
    {
        if (IsHost) return;
        UltimateCharge = charge;
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
        if (IsHost) return;
        GameObject projectileClone = NetworkManager.Singleton.SpawnManager.SpawnedObjects[projectileId].gameObject;
        projectileClone.GetComponent<Spawnables>().instantiatingArm = gameObject.GetComponent<Arm>();
    }

    public void FireProjectile(GameObject projectileClone, float force)  // SERVER ONLY
    {
        // FIRE PROJECTILE ON SERVER
        // Network Transform included - Synced to all clients
        Rigidbody2D rb = projectileClone.GetComponent<Rigidbody2D>();
        rb.AddForce(shootPoint.transform.up * force, ForceMode2D.Impulse);
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

    [ServerRpc(RequireOwnership = false)]
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

    public void ShakeCameraSkill()   // SERVER
    {
        NetworkManager.Singleton.ConnectedClients[OwnerClientId].PlayerObject.GetComponent<PlayerController>().ShakeCameraClientRpc(skillCameraShakeIntensity, skillCameraShakeDuration, new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { OwnerClientId }
            }
        });
    }

    public void ShakeCameraUltimate()   // SERVER
    {
        NetworkManager.Singleton.ConnectedClients[OwnerClientId].PlayerObject.GetComponent<PlayerController>().ShakeCameraClientRpc(ultimateCameraShakeIntensity, ultimateCameraShakeDuration, new ClientRpcParams
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

    public void UpgradeArm()            // SERVER ONLY      TODO: Add cost of upgrade
    {
        if (armLevel.Value == ArmLevel.Default)
        {
            armLevel.Value = ArmLevel.Upgraded;
            OnUpgraded();
        }
        else if (armLevel.Value == ArmLevel.Upgraded)
        {
            armLevel.Value = ArmLevel.Max;
            OnMax();
        }
        else { Debug.Log("Arm Not Upgradable"); }
    }

    public virtual void OnUpgraded() { }
    public virtual void OnMax() { }

    [ServerRpc(RequireOwnership = false)]
    public void UpgradeArmServerRpc()
    {
        //Logger.Instance.LogInfo("Upgraded Arm");
        UpgradeArm();
    }

    public enum WeaponState
    {
        Idle,
        BasicAttack,
        SkillAttack,
        UltimateAttack
    }

    public enum ArmType
    {
        Offense,
        Defense,
        Support
    }
}

public enum ArmLevel
{
    Default,
    Upgraded,
    Max
}