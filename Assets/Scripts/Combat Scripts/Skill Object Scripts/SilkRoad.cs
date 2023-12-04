using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;

public class SilkRoad : SkillObject
{
    public float stunDuration = 5f; // Duration of stun effect
    public float countdownTimer = 8f; // Countdown timer

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

    // TO FIX: WHY IF PLAYER = TEAMID 1, THEY CREATE A TEAMID = 0 SILK ROAD?
    // Although it does not stun them.
    public override void TriggerEnter2DLogic(Collider2D other)
    {
        // Only affect enemies
        PlayerController playerController = other.GetComponent<PlayerController>();
        if (playerController != null && !playersToStun.Contains(playerController) && playerController.teamId.Value != teamId.Value)
        {
            playerController.AdjustMovementSpeedServerRpc(playerController.MoveSpeed / 2f); // Halve MoveSpeed
            playerController.AdjustDamageTakenScaleServerRpc(playerController.DamageTakenScale * 2f); // Double Damage Taken
            playersToStun.Add(playerController);
            Debug.Log("ENTER COUNT: " + playersToStun.Count);
            Debug.Log("ENTER TEAMID: " + teamId.Value);
        }
    }

    public override void TriggerExit2DLogic(Collider2D other)
    {
        // Only affect enemies
        PlayerController playerController = other.GetComponent<PlayerController>();
        if (playerController != null && playersToStun.Contains(playerController) && playerController.teamId.Value != teamId.Value)
        {
            playerController.AdjustMovementSpeedServerRpc(playerController.MoveSpeed * 2f); // Reset MoveSpeed
            playerController.AdjustDamageTakenScaleServerRpc(playerController.DamageTakenScale / 2f); // Reset Damage Taken
            playersToStun.Remove(playerController);
            Debug.Log("EXIT COUNT: " + playersToStun.Count);
            Debug.Log("EXIT TEAMID: " + teamId.Value);

        }
    }
    private void ApplyStunToPlayers()
    {
        foreach (PlayerController playerController in playersToStun)
        {
            if (playerController != null)
            {
                playerController.RequestStunServerRpc(stunDuration);
                playerController.AdjustMovementSpeedServerRpc(playerController.MoveSpeed * 2f); // Reset MoveSpeed
                playerController.AdjustDamageTakenScaleServerRpc(playerController.DamageTakenScale / 2f); // Reset Damage Taken
            }
        }

        playersToStun.Clear(); // Clear the list after applying stun
    }

}