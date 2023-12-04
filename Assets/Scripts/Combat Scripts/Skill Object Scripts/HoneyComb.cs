using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;

public class HoneyComb : SkillObject
{
    [System.NonSerialized] public float countdownTimer; // Countdown timer
    [System.NonSerialized] public List<PlayerController> alliesToHeal = new List<PlayerController>();
    private float chargeTimer = 0f;
    public float healingValue = 10f;
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
            ally.healingPerSecond -= healingValue; // Reset Healing Per Second
        }

        foreach (PlayerController enemy in enemiestoSlow)
        {
            enemy.MoveSpeed *= 2f; // Reset MoveSpeed
        }
    }

    public override void TriggerEnter2DLogic(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null && playerController.teamId.Value == teamId.Value)
            {
                if (!playerController.interactingWithHoneyComb && !alliesToHeal.Contains(playerController))
                    alliesToHeal.Add(playerController);
                playerController.interactingWithHoneyComb = true;
                Debug.Log("HoneyComb Trigger Enter: " + playerController.interactingWithHoneyComb);
                playerController.healingPerSecond += healingValue;
            }
            else if (playerController != null && playerController.teamId.Value != teamId.Value)
            {
                if (!enemiestoSlow.Contains(playerController))
                {
                    playerController.MoveSpeed /= 2f; // Halve MoveSpeed
                    enemiestoSlow.Add(playerController);
                }
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
            playerController.interactingWithHoneyComb = true;
            Debug.Log("HoneyComb Trigger Enter: " + playerController.interactingWithHoneyComb);
            playerController.healingPerSecond -= healingValue; // Reset Healing Per Second
        }
        else if (playerController != null && playerController.teamId.Value != teamId.Value)
        {
            if (enemiestoSlow.Contains(playerController))
            {
                playerController.MoveSpeed *= 2f; // Reset MoveSpeed
                enemiestoSlow.Remove(playerController);
            }
        }
    }


}