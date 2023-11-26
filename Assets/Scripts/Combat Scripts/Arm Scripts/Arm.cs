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
            foreach (var playerClientId in NetworkManager.Singleton.ConnectedClientsIds)
            {

                Vector3 otherPlayerPosition = NetworkManager.Singleton.ConnectedClients[playerClientId].PlayerObject.transform.position;

                //CastBasicAttackSFXClientRpc(new ClientRpcParams
                CastBasicAttackSFXClientRpc(otherPlayerPosition, new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[] { playerClientId }
                    }
                });
            }
            //audioSource.PlayOneShot(basicAttackSFX, 1f);
        }
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
            Logger.Instance.LogInfo("distance:" + distance);

            float volumeRatio = Mathf.Clamp(1 - (distance / maxDistance), 0, 1);
            float volume = Mathf.Pow(volumeRatio, volumeExponent);
            Logger.Instance.LogInfo("volume:" + volume);

            audioSource.PlayOneShot(basicAttackSFX, volume);

            //audioSource.PlayOneShot(basicAttackSFX);
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
