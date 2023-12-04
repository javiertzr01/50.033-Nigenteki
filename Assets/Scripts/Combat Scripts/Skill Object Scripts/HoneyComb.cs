using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoneyComb : SkillObject
{
    public float countdownTimer; // Countdown timer
    public float healingValue = 10f;
    public List<PlayerController> alliesToHeal = new List<PlayerController>();
    private float chargeTimer = 0f;
    private List<PlayerController> enemiestoSlow = new List<PlayerController>();

    private void Update()
    {
        if (countdownTimer > 0f)
        {
            countdownTimer -= Time.deltaTime;
            if (countdownTimer <= 0f)
            {
                Cleanup();
                DestroyServerRpc();
            }
        }

        // Check if there are any allies to heal
        if (alliesToHeal.Count > 0)
        {
            // Increment the timer
            chargeTimer += Time.deltaTime;

            // Check if a second has passed
            if (chargeTimer >= 1f)
            {
                // Charge the ultimate
                instantiatingArm.ChargeUltimateServerRpc(healingValue, 5);

                // Reset the timer
                chargeTimer = 0f;
            }
        }
        else
        {
            // Reset the timer if there are no allies
            chargeTimer = 0f;
        }
    }

    private void Cleanup()
    {
        foreach (PlayerController ally in alliesToHeal)
        {
            ally.interactingWithHoneyComb = false;
            ally.healingPerSecond = ally.defaultHealingPerSecond; // Reset Healing Per Second
        }

        foreach (PlayerController enemy in enemiestoSlow)
        {
            enemy.AdjustMovementSpeedServerRpc(enemy.defaultMoveSpeed);
        }
    }

    public override void TriggerEnter2DLogic(Collider2D other)
    {
        PlayerController playerController = other.GetComponent<PlayerController>();
        if (playerController != null && playerController.teamId.Value == teamId.Value)
        {
            if (!playerController.interactingWithHoneyComb && !alliesToHeal.Contains(playerController))
                alliesToHeal.Add(playerController);
            playerController.interactingWithHoneyComb = true;
            Debug.Log("HoneyComb Trigger Enter: " + playerController.interactingWithHoneyComb);
            playerController.healingPerSecond = playerController.defaultHealingPerSecond + healingValue;
        }
        else if (playerController != null && playerController.teamId.Value != teamId.Value)
        {
            if (!enemiestoSlow.Contains(playerController))
            {
                playerController.AdjustMovementSpeedServerRpc(playerController.defaultMoveSpeed / 2);
                enemiestoSlow.Add(playerController);
            }
        }
    }

    public override void TriggerStay2DLogic(Collider2D other)
    {
    }

    public override void TriggerExit2DLogic(Collider2D other)
    {
        PlayerController playerController = other.GetComponent<PlayerController>();
        if (playerController != null && playerController.teamId.Value == teamId.Value)
        {
            if (playerController.interactingWithHoneyComb && alliesToHeal.Contains(playerController))
                alliesToHeal.Remove(playerController);
            playerController.interactingWithHoneyComb = false;
            Debug.Log("HoneyComb Trigger Exit: " + playerController.interactingWithHoneyComb);
            playerController.healingPerSecond = playerController.defaultHealingPerSecond; // Reset Healing Per Second
        }
        else if (playerController != null && playerController.teamId.Value != teamId.Value)
        {
            if (enemiestoSlow.Contains(playerController))
            {
                playerController.AdjustMovementSpeedServerRpc(playerController.defaultMoveSpeed);
                enemiestoSlow.Remove(playerController);
            }
        }
    }


}