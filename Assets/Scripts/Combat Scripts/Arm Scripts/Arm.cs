using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public abstract class Arm : NetworkBehaviour, INetworkSerializable
{
    public ArmVariables armVariable; // Ensure ArmVariables is serializable if it contains network-relevant data
    public NetworkVariable<WeaponState> networkWeaponState = new NetworkVariable<WeaponState>();
    public ArmType armType;

    // Serialize references by ID or some other network-friendly method, not directly
    [SerializeField]
    protected List<GameObject> projectiles; // Consider serializing relevant state information instead of GameObjects

    [SerializeField]
    protected GameObject shootPoint; // Same as above, consider what needs to be synchronized
    [SerializeField]
    private Animator animator;
    protected GameObject basicProjectile;
    private float _ultimateCharge;

    protected AudioSource audioSource0;
    protected AudioSource audioSource1;
    protected AudioSource audioSource2;

    public AudioClip basicAttackSFX;   // Assign this in the Inspector
    public AudioClip skillSFX;   // Assign this in the Inspector
    public AudioClip ultimateSFX;   // Assign this in the Inspector

    public float basicAttackCameraShakeIntensity;
    public float basicAttackCameraShakeDuration;
    public float skillCameraShakeIntensity;
    public float skillCameraShakeDuration; 
    public float ultimateCameraShakeIntensity;
    public float ultimateCameraShakeDuration;

    protected void Awake()
    {
        audioSource0 = gameObject.AddComponent<AudioSource>();
        audioSource1 = gameObject.AddComponent<AudioSource>();
        audioSource2 = gameObject.AddComponent<AudioSource>();
    }

    protected virtual void Start()
    {
        Initialize();
    }

    public virtual void Initialize()
    {
        basicProjectile = projectiles[0];
    }

    private float _skillCoolDown;

    public float SkillCoolDown
    {
        get => _skillCoolDown;
        set => _skillCoolDown = value;
    }

    // The basic attack method
    [ServerRpc(RequireOwnership = false)]
    public virtual void CastBasicAttackServerRpc(ServerRpcParams serverRpcParams = default) { }

    [ClientRpc]
    public virtual void CastBasicAttackClientRpc(ClientRpcParams clientRpcParams = default) { }

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

    [ServerRpc(RequireOwnership = false)]
    public virtual void CastSkillServerRpc(ServerRpcParams serverRpcParams = default) { }

    [ClientRpc]
    public virtual void CastSkillClientRpc(ClientRpcParams clientRpcParams = default) { }

    [ServerRpc(RequireOwnership = false)]
    public virtual void CastUltimateServerRpc(ServerRpcParams serverRpcParams = default) { }

    [ClientRpc]
    public virtual void CastUltimateClientRpc(ClientRpcParams clientRpcParams = default) { }

    public void ChargeUltimate(float charge, float divisor)
    {
        if (divisor < 1)
        {
            divisor = 1;
        }
        UltimateCharge += (charge / divisor);
        Debug.Log(armVariable.armName + " Ultimate Charge: " + UltimateCharge);
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateWeaponAnimatorServerRpc(WeaponState newState)
    {
        networkWeaponState.Value = newState;
    }

    public void UpdateWeaponAnimator()
    {
        if (networkWeaponState.Value == WeaponState.Idle)
        {
            animator.SetBool("isBasicAttack", true);
        }
        else if (networkWeaponState.Value == WeaponState.BasicAttack)
        {
            animator.SetBool("isBasicAttack", false);
        }
    }

    public float UltimateCharge
    {
        get => _ultimateCharge;
        set => _ultimateCharge = Mathf.Clamp(value, 0, 100);
    }

    public virtual bool HaveSkillCharges()
    {
        return false; // Default implementation, since Arm doesn't have skill charges
    }

    public virtual int GetSkillCharges()
    {
        return 0; // Default implementation
    }

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

    public enum ArmType
    {
        Offense,
        Defense,
        Support
    }
}
