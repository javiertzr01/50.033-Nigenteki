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
        TakeDamageServerRpc(other.gameObject.GetComponent<Projectile>().Damage, OwnerClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public override void TakeDamageServerRpc(float damage, ulong clientId)
    {
        if (OwnerClientId != clientId) return;

        ShieldHealth -= damage;
        Logger.Instance.LogInfo("BEETLE SKILL HP: " + ShieldHealth);
        if (ShieldHealth <= 0)
        {
            Logger.Instance.LogInfo("BEETLE SKILL DESTROYED");
            DestroyShield();
        }
    }

}