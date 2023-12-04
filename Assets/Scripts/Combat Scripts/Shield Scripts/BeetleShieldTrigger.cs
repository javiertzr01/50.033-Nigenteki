using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
public class BeetleShieldTrigger : ShieldTrigger
{
    Beetle arm;

    [SerializeField]
    private Collider2D shieldCollider;
    [SerializeField]
    private SpriteRenderer shieldSprite;
    private float shieldRegenTimer;


    protected AudioSource audioSource0;
    protected AudioSource audioSource1;
    protected AudioSource audioSource2;
    protected AudioSource audioSource3;

    public AudioClip activateShieldSFX;
    public AudioClip deactivateShieldSFX;
    public AudioClip destroyedShieldSFX;
    public AudioClip disabledOptionSFX;
    public AudioClip takeDamageSFX;

    private bool isShieldDestroyedCompleted = false;



    protected void Awake()
    {
        audioSource0 = gameObject.AddComponent<AudioSource>();
        audioSource1 = gameObject.AddComponent<AudioSource>();
        audioSource2 = gameObject.AddComponent<AudioSource>();
        audioSource3 = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        shieldCollider = gameObject.GetComponent<BoxCollider2D>();
        shieldSprite = gameObject.GetComponentInChildren<SpriteRenderer>();

        shieldRegenTimer = 0f;
        isShieldActive.Value = true;
        Destroyed = false;
        ShieldHealth = instantiatingArm.GetComponent<Arm>().armVariable.shieldMaxHealth;

        // Initialize the shield's state based on isShieldActive value
        shieldCollider.enabled = isShieldActive.Value;
        shieldSprite.enabled = isShieldActive.Value;
    }

    [ServerRpc(RequireOwnership = false)]
    public void CastShieldSFXServerRpc(int shieldTypeIndex, ServerRpcParams serverRpcParams = default)
    {
        PlayAudioForAllClients(shieldTypeIndex);
    }

    // Server-side method to loop and play audio
    public void PlayAudioForAllClients(int shieldTypeIndex, ClientRpcParams clientRpcParams = default)
    {
        AudioSource audioSource;
        AudioClip audioClip;
        SetAudioSourceClips(shieldTypeIndex, out audioSource, out audioClip);

        if (audioClip != null)
        {
            foreach (var playerClientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                Vector3 otherPlayerPosition = NetworkManager.Singleton.ConnectedClients[playerClientId].PlayerObject.transform.position;

                PlayAudioClientRpc(shieldTypeIndex, otherPlayerPosition, new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[] { playerClientId }
                    }
                });
            }
        }
    }

    protected virtual void SetAudioSourceClips(int shieldTypeIndex, out AudioSource audioSource, out AudioClip audioClip)
    {
        switch (shieldTypeIndex)
        {
            case 4:
                audioSource = audioSource3;
                audioClip = takeDamageSFX;
                break;
            case 3:
                audioSource = audioSource3;
                audioClip = disabledOptionSFX;
                break;
            case 2:
                audioSource = audioSource2;
                audioClip = destroyedShieldSFX;
                break;
            case 1:
                audioSource = audioSource1;
                audioClip = deactivateShieldSFX;
                break;
            default:
            case 0:
                audioSource = audioSource0;
                audioClip = activateShieldSFX;
                break;
        }
    }

    [ClientRpc]
    public void PlayAudioClientRpc(int shieldTypeIndex, Vector3 otherPlayerPosition, ClientRpcParams clientRpcParams = default)
    {
        PlayAudio(shieldTypeIndex, otherPlayerPosition);
    }


    // Method to play audio with given AudioSource and AudioClip
    protected void PlayAudio(int shieldTypeIndex, Vector3 otherPlayerPosition)
    {
        AudioSource audioSource;
        AudioClip audioClip;
        SetAudioSourceClips(shieldTypeIndex, out audioSource, out audioClip);

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

    // Network variable to keep track of the shield's activation state
    public NetworkVariable<bool> isShieldActive = new NetworkVariable<bool>(false);

    [ServerRpc(RequireOwnership = false)]
    public void ToggleShieldServerRpc(ServerRpcParams serverRpcParams = default)
    {
        // Toggle the shield's activation state
        isShieldActive.Value = !isShieldActive.Value;

        if (Destroyed)
        {
            //Audio Player
            //Enable - 0; DisableShield - 1; Destroyed - 2; DisabledOption - 3;
            CastShieldSFXServerRpc(3);
        }
        else if (isShieldActive.Value)
        {
            //Audio Player
            //Enable - 0; DisableShield - 1; Destroyed - 2; DisabledOption - 3;
            CastShieldSFXServerRpc(0);
        }
        else
        {
            //Audio Player
            //Enable - 0; DisableShield - 1; Destroyed - 2; DisabledOption - 3;
            CastShieldSFXServerRpc(1);
        }

        // Call the client RPC to update the shield state on all clients
        ToggleShieldClientRpc(isShieldActive.Value);
    }

    [ClientRpc]
    void ToggleShieldClientRpc(bool isActive, ClientRpcParams clientRpcParams = default)
    {
        // Update the shield's collider and sprite renderer based on the received state
        shieldCollider.enabled = isActive;
        shieldSprite.enabled = isActive;
    }


    void Update()
    {
        // Check if the shield is not active
        if (!isShieldActive.Value)
        {
            shieldRegenTimer += Time.deltaTime;

            // Start regeneration after 3 seconds of inactivity
            if (shieldRegenTimer >= 3.0f)
            {
                RegenerateShield();
            }
        }
        else
        {
            // Reset the timer if the shield is active or not destroyed
            shieldRegenTimer = 0f;
        }
    }

    private void RegenerateShield()
    {
        if (ShieldHealth < instantiatingArm.GetComponent<Arm>().armVariable.shieldMaxHealth)
        {
            ShieldHealth += 50f * Time.deltaTime; // Regenerate HP per second
            if (ShieldHealth >= instantiatingArm.GetComponent<Arm>().armVariable.shieldMaxHealth)
            {
                ShieldHealth = instantiatingArm.GetComponent<Arm>().armVariable.shieldMaxHealth;
                Destroyed = false; // Mark shield as not destroyed
            }

            Logger.Instance.LogInfo("Shield Regen HP: " + ShieldHealth);

            // Do not update the visual state here, keep the shield invisible and non-colliding during regeneration
            UpdateShieldStatusClientRpc(ShieldHealth, Destroyed, new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = NetworkManager.Singleton.ConnectedClientsList.Select(c => c.ClientId).ToArray()
                }
            });
        }
    }


    public override void TriggerEnter2DLogic(Collider2D other)
    {
    }

    [ServerRpc(RequireOwnership = false)]
    public override void TakeDamageServerRpc(float damage, ulong clientId)
    {
        if (OwnerClientId != clientId) return;

        ShieldHealth -= damage;
        if (ShieldHealth <= 0f)
        {
            ShieldHealth = 0f;
            Destroyed = true;
        }
        Logger.Instance.LogInfo("BEETLE SHIELD HP: " + ShieldHealth);


        // Update clients about the shield's status, including if it's destroyed
        UpdateShieldStatusClientRpc(ShieldHealth, Destroyed, new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = NetworkManager.Singleton.ConnectedClientsList.Select(c => c.ClientId).ToArray()
            }
        });
    }

    [ClientRpc]
    private void UpdateShieldStatusClientRpc(float health, bool destroyed, ClientRpcParams clientRpcParams = default)
    {
        ShieldHealth = health;
        Destroyed = destroyed;
        isShieldActive.Value = (ShieldHealth > 0) && !destroyed;


        // Update the shield's visual or physical state on clients
        if (isShieldActive.Value && !destroyed)
        {
            // If the shield is not destroyed, you can update its state as needed
            // For example, you might want to change the appearance to indicate damage but not disable it completely
            // Update the shield's visual or physical state on clients
            shieldCollider.enabled = true;
            shieldSprite.enabled = true;

            //Audio Player
            //Enable - 0; DisableShield - 1; Destroyed - 2; DisabledOption - 3; Damage - 4;
            CastShieldSFXServerRpc(4);
            isShieldDestroyedCompleted = false;


        }
        else
        {

            // If the shield is destroyed, disable collider and sprite
            shieldCollider.enabled = false;
            shieldSprite.enabled = false;

            //Audio Player
            //Enable - 0; DisableShield - 1; Destroyed - 2; DisabledOption - 3;
            if (!isShieldDestroyedCompleted && destroyed)
            {
                CastShieldSFXServerRpc(2);
                isShieldDestroyedCompleted = true;
            }
        }
    }

    public void ResetShieldHealth()
    {
        ShieldHealth = instantiatingArm.GetComponent<Arm>().armVariable.shieldMaxHealth;
        Destroyed = false;
        isShieldActive.Value = false; // Optionally reset the active state
        Logger.Instance.LogInfo("Reset Beetle Shield Health to: " + ShieldHealth);

        
        UpdateShieldStatusClientRpc(ShieldHealth, Destroyed, new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = NetworkManager.Singleton.ConnectedClientsList.Select(c => c.ClientId).ToArray()
            }
        });
    }

}