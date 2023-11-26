using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public abstract class Arm : NetworkBehaviour, INetworkSerializable
{
    public ArmVariables armVariable; // Ensure ArmVariables is serializable if it contains network-relevant data
    public NetworkVariable<WeaponState> networkWeaponState = new NetworkVariable<WeaponState>();

    // Serialize references by ID or some other network-friendly method, not directly
    [SerializeField]
    protected List<GameObject> projectiles; // Consider serializing relevant state information instead of GameObjects

    [SerializeField]
    protected GameObject shootPoint; // Same as above, consider what needs to be synchronized
    [SerializeField]
    private Animator animator;
    protected GameObject basicProjectile;
    private float _ultimateCharge;
    protected AudioSource audioSource;
    public AudioClip basicAttackSFX;   // Assign this in the Inspector
    public AudioClip skillSFX;   // Assign this in the Inspector
    public AudioClip ultimateSFX;   // Assign this in the Inspector


    void Start()
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

    [ServerRpc(RequireOwnership = false)]
    public void CastBasicAttackSFXServerRpc(ServerRpcParams serverRpcParams = default)
    {
        if (basicAttackSFX != null && audioSource != null)
        {
            foreach (var player in NetworkManager.Singleton.ConnectedClientsIds)
            {
                CastBasicAttackSFXClientRpc(new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[] { player }
                    }
                });
            }
            audioSource.PlayOneShot(basicAttackSFX);
        }
    }

    [ClientRpc]
    public void CastBasicAttackSFXClientRpc(ClientRpcParams clientRpcParams = default)
    {
        if (basicAttackSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(basicAttackSFX);
        }
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
