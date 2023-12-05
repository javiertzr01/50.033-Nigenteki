using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Locust : Arm
{
    private Color originalColor;
    private NetworkVariable<Color> syncedColor = new NetworkVariable<Color>(new Color(1, 1, 1, 1)); // default color
    private SpriteRenderer playerSprite;
    GameObject ProjectileSpawned;
    private bool upgraded = false;
    [SerializeField]
    private bool ulted = false;


    private void Start()
    {
        playerSprite = gameObject.GetComponentInParent<SpriteRenderer>();
        if (playerSprite != null)
        {
            originalColor = playerSprite.color;
            syncedColor.Value = playerSprite.color;
        }
        else
        {
            Debug.LogError("SpriteRenderer not found!");
        }

        // Subscribe to the color change event
        syncedColor.OnValueChanged += OnColorChanged;
    }

    public override void OnUpgraded()
    {
        base.OnUpgraded();
        upgraded = true;
        Logger.Instance.LogInfo($"Increased Attack Range");
    }

    public override void OnMax()
    {
        base.OnMax();
        Logger.Instance.LogInfo($"Increased Attack Damage");
    }

    public override void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        syncedColor.OnValueChanged -= OnColorChanged;
        base.OnDestroy();
    }

    private void OnColorChanged(Color oldColor, Color newColor)
    {
        Debug.Log($"Color changed from {oldColor} to {newColor}");
        if (playerSprite != null)
        {
            playerSprite.color = newColor;
        }
        else
        {
            Debug.LogError("Failed to apply color change: SpriteRenderer is null.");
        }
    }

    private void Update()
    {
        // Apply synchronized color
        if (playerSprite != null && syncedColor.Value != playerSprite.color)
        {
            playerSprite.color = new Color(syncedColor.Value.r, syncedColor.Value.g, syncedColor.Value.b, playerSprite.color.a);
        }

        // Update the skill charge timer
        if (SkillCharges < maxSkillCharges)
        {
            SkillCoolDown -= Time.deltaTime;
            if (SkillCoolDown <= 0)
            {
                // Increment skillCharges when the timer reaches 0
                SkillCharges++;
                Debug.Log("Increment Locust Skill Charge: " + SkillCharges);
                // Reset the skill charge timer
                SkillCoolDown = armVariable.skillCoolDown;
            }
        }
        // Update the ultimate timer
        if (ulted)
        {
            countdownTimer -= Time.deltaTime;
            if (countdownTimer <= 0f)
            {
                ulted = false; // Reset the ulted flag when the timer reaches 0
                gameObject.GetComponentInParent<PlayerController>().ToggleImmuneStunServerRpc(false);
                Debug.Log("LOCUST ULT: Expired");
                ShakeCamera();
                SetColor(originalColor); // Reset color


                countdownTimer = armVariable.ultimateDuration; // Reset the timer for the next ultimate
            }
        }
    }

    private void SetColor(Color newColor)
    {
        if (IsServer) // Ensure the server is making the change
        {
            syncedColor.Value = newColor;
        }
        else
        {
            Debug.LogWarning("SetColor called from a non-server instance.");
        }

    }

    public override void SetProjectiles()
    {
        basicProjectile = projectiles[0];
        ultimateProjectile = projectiles[1];
    }

    [ServerRpc(RequireOwnership = false)]
    public override void CastBasicAttackServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;

        if (OwnerClientId != clientId) return;

        if (Time.time >= nextBasicFireTime)
        {
            // Limit number of projectiles to 1 UNLESS ULTED
            if (ulted || ProjectileSpawned == null || upgraded)
            {
                //Logger.Instance.LogInfo($"Cast Basic Attack ServerRpc called by {clientId}");

                // Instantiate the type of Projectile
                if (ulted)
                {
                    ProjectileSpawned = SpawnProjectile<Projectile>(clientId, ultimateProjectile, shootPoint);

                }
                else
                {
                    ProjectileSpawned = SpawnProjectile<Projectile>(clientId, basicProjectile, shootPoint);
                }
                FireProjectile(ProjectileSpawned, armVariable.baseForce);

                //Audio Player
                int attackTypeIndex = 0; //Basic - 0; Skill - 1; Ultimate - 2;
                CastAttackSFXServerRpc(attackTypeIndex, serverRpcParams);

                // Set the nextBasicFireTime
                if (ulted)
                {
                    nextBasicFireTime = Time.time + armVariable.ultimateFireRate;
                }
                else
                {
                    nextBasicFireTime = Time.time + armVariable.baseFireRate;
                }

                CastBasicAttackClientRpc(new ClientRpcParams        // REMOVE : This just notifies the client
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[] { clientId }
                    }
                });
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public override void CastSkillServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;

        if (OwnerClientId != clientId) return;
        Debug.Log("Skill casted");

        if (ulted || SkillCharges > 0)
        {
            //Audio Player
            int attackTypeIndex = 1; //Basic - 0; Skill - 1; Ultimate - 2;
            CastAttackSFXServerRpc(attackTypeIndex, serverRpcParams);


            // Trigger the dash on the client
            gameObject.GetComponentInParent<PlayerController>().TriggerDashClientRpc(new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            });

            // Decrease the number of available skill charges
            if (!ulted)
            {
                SkillCharges--;
                Debug.Log("Decrease Locust Skill Charge: " + SkillCharges);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public override void CastUltimateServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;

        if (OwnerClientId != clientId) return;


        if (UltimateCharge >= 100f)
        {
            //Logger.Instance.LogInfo($"Cast Ultimate ServerRpc called by {clientId}");

            Debug.Log("LOCUST ULTIMATE: Casting");
            ShakeCameraUltimate();
            ResetUltimateCharge();
            ulted = true;
            gameObject.GetComponentInParent<PlayerController>().ToggleImmuneStunServerRpc(true);
            Color tint = new Color(0.5f, 1f, 0.5f, 1f); // Light Green hue
            SetColor(tint);

            //Audio Player
            int attackTypeIndex = 2; //Basic - 0; Skill - 1; Ultimate - 2;
            CastAttackSFXServerRpc(attackTypeIndex, serverRpcParams);

            // Cast the Ultimate ClientRpc
            CastUltimateClientRpc(new ClientRpcParams               // REMOVE : This just notifies the client
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            });
        }
    }
}