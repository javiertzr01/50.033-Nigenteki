using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public abstract class Arm : NetworkBehaviour
{
    public ArmVariables armVariable;

    public NetworkVariable<WeaponState> networkWeaponState = new NetworkVariable<WeaponState>();

    [SerializeField]
    protected List<GameObject> projectiles;

    [SerializeField]
    protected GameObject shootPoint;

    [SerializeField]
    private Animator animator;

    protected GameObject basicProjectile;
    private float _ultimateCharge;

    protected AudioSource audioSource;
    public AudioClip basicAttackSFX;   // Assign this in the Inspector
    public AudioClip skillSFX;   // Assign this in the Inspector
    public AudioClip ultimateSFX;   // Assign this in the Inspector


    void Awake()
    {
        Initialize();
    }

    // Initialize the arm with its variable settings
    public virtual void Initialize()
    {
        // Initialize arm with the variables from armVariable.
        // E.g., set the arm's sprite, attack power, etc.
        basicProjectile = projectiles[0];
    }

    // The basic attack method
    [ServerRpc(RequireOwnership = false)]
    public virtual void CastBasicAttackServerRpc(ServerRpcParams serverRpcParams = default) { }
    [ClientRpc]
    public virtual void CastBasicAttackClientRpc(ClientRpcParams clientRpcParams = default) { }

    [ServerRpc(RequireOwnership = false)]
    public void CastBasicAttackSFXServerRpc(ServerRpcParams serverRpcParams = default)
    {
        if(basicAttackSFX != null && audioSource != null)
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

    // The skill method
    public abstract void CastSkill();

    // The ultimate skill method
    public abstract void CastUltimate();

    // Higher the Divisor, the slower the charging rate
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
        get
        {
            return _ultimateCharge;
        }
        set
        {
            if (value >= 100)
            {
                _ultimateCharge = 100;
            }
            else
            {
                _ultimateCharge = value;
            }

        }
    }

    public enum WeaponState
    {
        Idle,
        BasicAttack,
        SkillAttack,
        UltimateAttack
    }    
}
