using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
public class BeetleShieldTrigger : ShieldTrigger
{
    Beetle arm;

    // Network variable to keep track of the shield's activation state
    public NetworkVariable<bool> isShieldActive = new NetworkVariable<bool>(false);
    public NetworkVariable<bool> isAttackingNoShield = new NetworkVariable<bool>(false);
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

    void Awake()
    {
        shieldCollider = gameObject.GetComponent<BoxCollider2D>();
        shieldSprite = gameObject.GetComponentInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();
        animator.enabled = false;
        audioSource0 = gameObject.AddComponent<AudioSource>();
        audioSource1 = gameObject.AddComponent<AudioSource>();
        audioSource2 = gameObject.AddComponent<AudioSource>();
        audioSource3 = gameObject.AddComponent<AudioSource>();
    }

    public override void Start()
    {
        base.Start();
        shieldRegenTimer = 0f;
        Destroyed = false;
        arm = transform.GetComponentInParent<Beetle>();

        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Color tint = teamId.Value == 0 ? Color.red : Color.blue;
        spriteRenderer.color = new Color(tint.r, tint.g, tint.b, spriteRenderer.color.a);


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
    void Update()
    {
        // Check if the shield is not active
        if (!isShieldActive.Value)
        {
            if (isAttackingNoShield.Value)
            {
                shieldRegenTimer = 0;
                ToggleAttackingNoShieldServerRpc(false);
            }
            shieldRegenTimer += Time.deltaTime;

            // Start regeneration after 3 seconds of inactivity
            if (shieldRegenTimer >= 3.0f)
            {
                RegenerateShieldServerRpc();
            }
        }
        else
        {
            // Reset the timer if the shield is active or not destroyed
            shieldRegenTimer = 0f;
            if (ShieldHealth < 0.25f * shieldMaxHealth)
            {
                animator.enabled = true;
            }
            else if (animator.enabled)
            {
                animator.enabled = false;
            }
        }
        if ((shieldCollider.enabled == isShieldActive.Value) && (shieldSprite.enabled == isShieldActive.Value)) { return; }
        else
        {
            shieldCollider.enabled = isShieldActive.Value;
            shieldSprite.enabled = isShieldActive.Value;
        }
    }

    public void ToggleShield()      // SERVER ONLY
    {
        // Toggle the shield's activation state
        isShieldActive.Value = !isShieldActive.Value;
        if (Destroyed)
        {
            //Audio Player
            //Enable - 0; DisableShield - 1; Destroyed - 2; DisabledOption - 3;
            PlayAudioForAllClients(3);
        }
        else if (isShieldActive.Value)
        {
            //Audio Player
            //Enable - 0; DisableShield - 1; Destroyed - 2; DisabledOption - 3;
            PlayAudioForAllClients(0);
        }
        else
        {
            //Audio Player
            //Enable - 0; DisableShield - 1; Destroyed - 2; DisabledOption - 3;
            PlayAudioForAllClients(1);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ToggleAttackingNoShieldServerRpc(bool value)
    {
        isAttackingNoShield.Value = value;
    }

    [ServerRpc(RequireOwnership = false)]
    public void RegenerateShieldServerRpc()
    {
        if (ShieldHealth < shieldMaxHealth)
        {
            ShieldHealth += 50f * Time.deltaTime; // Regenerate HP per second
            if (ShieldHealth >= shieldMaxHealth)
            {
                ShieldHealth = shieldMaxHealth;
            }

            if (ShieldHealth >= 0.5 * shieldMaxHealth)
            {
                Destroyed = false; // Mark shield as not destroyed
            }
            // Do not update the visual state here, keep the shield invisible and non-colliding during regeneration
            UpdateShieldStatusClientRpc(ShieldHealth, Destroyed);
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
            isShieldActive.Value = false;
            //Enable - 0; DisableShield - 1; Destroyed - 2; DisabledOption - 3; Damage - 4;
            PlayAudioForAllClients(2);
        }
        else
        {
            //Enable - 0; DisableShield - 1; Destroyed - 2; DisabledOption - 3; Damage - 4;
            PlayAudioForAllClients(4);
        }
        Logger.Instance.LogInfo("BEETLE SHIELD HP: " + ShieldHealth);


        // Update clients about the shield's status, including if it's destroyed
        UpdateShieldStatusClientRpc(ShieldHealth, Destroyed);
    }

    [ClientRpc]
    private void UpdateShieldStatusClientRpc(float health, bool destroyed)
    {
        if (IsHost) return;
        ShieldHealth = health;
        Destroyed = destroyed;
    }

    public void ResetShieldHealth()
    {
        ShieldHealth = shieldMaxHealth;
        Destroyed = false;
        isShieldActive.Value = false; // Optionally reset the active state
        Logger.Instance.LogInfo("Reset Beetle Shield Health to: " + ShieldHealth);
        UpdateShieldStatusClientRpc(ShieldHealth, Destroyed);
    }
}