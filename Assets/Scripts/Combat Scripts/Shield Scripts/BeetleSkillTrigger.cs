using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BeetleSkillTrigger : ShieldTrigger
{
    Beetle arm;

    [SerializeField]
    private float countdownTimer; // Countdown timer

    void Start()
    {
        arm = instantiatingArm.GetComponent<Beetle>();
        countdownTimer = arm.armVariable.skillDuration;
    }

    private void Update()
    {
        if (countdownTimer > 0f)
        {
            countdownTimer -= Time.deltaTime;
            if (countdownTimer <= 0f)
            {
                DestroyServerRpc();
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
        Logger.Instance.LogInfo("BEETLE SKILL HP: " + ShieldHealth);
        if (ShieldHealth <= 0)
        {
            Logger.Instance.LogInfo("BEETLE SKILL DESTROYED");
            DestroyServerRpc();
        }
    }

}