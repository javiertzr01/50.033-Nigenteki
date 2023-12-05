using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BeetleSkillTrigger : ShieldTrigger
{

    [SerializeField]
    private float skillDurationTimer; // Countdown timer

    public override void Start()
    {
        base.Start();
        skillDurationTimer = instantiatingArm.armVariable.skillDuration;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color tint = teamId.Value == 0 ? new Color(1f, 0.5f, 0.5f, 1f) : new Color(0.5f, 0.5f, 1f, 1f);  // Light hue of red and blue
        spriteRenderer.color = new Color(tint.r, tint.g, tint.b, spriteRenderer.color.a);
    }

    private void Update()
    {
        if (skillDurationTimer > 0f)
        {
            skillDurationTimer -= Time.deltaTime;
            if (skillDurationTimer <= 0f)
            {
                DestroyShieldServerRpc();
            }
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
        //Logger.Instance.LogInfo("BEETLE SKILL HP: " + ShieldHealth);
        if (ShieldHealth <= 0)
        {
            //Logger.Instance.LogInfo("BEETLE SKILL DESTROYED");
            DestroyShield();
        }
    }

}