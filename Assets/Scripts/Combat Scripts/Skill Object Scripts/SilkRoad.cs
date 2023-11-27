using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;

public class SilkRoad : SkillObject
{
    public float stunDuration = 5f; // Duration of stun effect
    private float countdownTimer = 5f; // Countdown timer

    private List<PlayerController> playersToStun = new List<PlayerController>();

    private void Update()
    {
        if (countdownTimer > 0f)
        {
            countdownTimer -= Time.deltaTime;
            if (countdownTimer <= 0f)
            {
                ApplyStunToPlayers();
                DestroyServerRpc();

            }
        }
    }

    public override void TriggerEnter2DLogic(Collider2D other)
    {
        // Only affect enemies
        PlayerController playerController = other.GetComponent<PlayerController>();
        if (playerController != null && !playersToStun.Contains(playerController) && playerController.teamId.Value != teamId.Value)
        {
            playerController.MoveSpeed /= 2f; // Halve MoveSpeed
            playerController.DamageTakenScale *= 2f; // Double Damage Taken
            playersToStun.Add(playerController);
        }
    }

    public override void TriggerExit2DLogic(Collider2D other)
    {
        // Only affect enemies
        PlayerController playerController = other.GetComponent<PlayerController>();
        if (playerController != null && playersToStun.Contains(playerController) && playerController.teamId.Value != teamId.Value)
        {
            playerController.MoveSpeed *= 2f; // Reset MoveSpeed
            playerController.DamageTakenScale /= 2f; // Reset Damage Taken
            playersToStun.Remove(playerController);
        }
    }
    private void ApplyStunToPlayers()
    {
        foreach (PlayerController playerController in playersToStun)
        {
            if (playerController != null)
            {
                playerController.RequestStunServerRpc(stunDuration);
                playerController.MoveSpeed *= 2f; // Reset Movespeed
                playerController.DamageTakenScale /= 2f; // Reset Damage Taken
            }
        }

        playersToStun.Clear(); // Clear the list after applying stun
    }

}