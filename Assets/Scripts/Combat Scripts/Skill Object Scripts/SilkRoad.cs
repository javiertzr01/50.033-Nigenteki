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
        PlayerController playerController = other.GetComponent<PlayerController>();
        if (playerController != null && !playersToStun.Contains(playerController))
        {
            playerController.MoveSpeed /= 2f; // Halve MoveSpeed
            playersToStun.Add(playerController);
        }
    }

    public override void TriggerExit2DLogic(Collider2D other)
    {
        PlayerController playerController = other.GetComponent<PlayerController>();
        if (playerController != null && playersToStun.Contains(playerController))
        {
            playerController.MoveSpeed *= 2f; // Reset MoveSpeed
            playersToStun.Remove(playerController);
        }
    }

    private void ApplyStunToPlayers()
    {
        foreach (PlayerController playerController in playersToStun)
        {
            playerController.MoveSpeed *= 2f; // Reset MoveSpeed
            if (playerController != null)
            {
                // Apply stun effect by calling ApplyStun from PlayerController
                playerController.ApplyStun(stunDuration);
            }
        }

        playersToStun.Clear(); // Clear the list of players to prevent duplicate stuns
    }

    public override void CollisionEnter2DLogic(Collision2D collider)
    {
        // throw new System.NotImplementedException();
    }


}