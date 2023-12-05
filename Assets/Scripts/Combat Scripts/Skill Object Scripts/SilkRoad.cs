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
    public float defaultMoveSpeed;
    private bool enterAdjustMoveSpeed = false;
    private bool exitAdjustMoveSpeed = false;

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

    protected override void ColorSpriteToTeamId()
    {
        Color tint = teamId.Value == 0 ? Color.red : Color.blue;  // Light hue of red and blue

        foreach (SpriteRenderer spriteRenderer in GetComponentsInChildren<SpriteRenderer>())
        {
            spriteRenderer.color = new Color(tint.r, tint.g, tint.b, spriteRenderer.color.a);
        }

    }


    public override void TriggerEnter2DLogic(Collider2D other)
    {
        if (enterAdjustMoveSpeed) return;
        enterAdjustMoveSpeed = true;
        // Only affect enemies
        PlayerController playerController = other.GetComponent<PlayerController>();
        if (playerController != null && !playersToStun.Contains(playerController) && playerController.teamId.Value != teamId.Value)
        {
            playerController.AdjustMovementSpeedServerRpc(playerController.defaultMoveSpeed / 2f); // Halve MoveSpeed
            playerController.AdjustDamageTakenScaleServerRpc(playerController.defaultDamageTakenScale * 2f); // Double Damage Taken
            playersToStun.Add(playerController);
            Debug.Log("ENTER COUNT: " + playersToStun.Count);
            Debug.Log("ENTER TEAMID: " + teamId.Value);
        }
        enterAdjustMoveSpeed = false;
    }

    public override void TriggerExit2DLogic(Collider2D other)
    {
        if (exitAdjustMoveSpeed) return;
        exitAdjustMoveSpeed = true;
        // Only affect enemies
        PlayerController playerController = other.GetComponent<PlayerController>();
        if (playerController != null && playersToStun.Contains(playerController) && playerController.teamId.Value != teamId.Value)
        {
            playerController.AdjustMovementSpeedServerRpc(playerController.defaultMoveSpeed); // Reset MoveSpeed
            playerController.AdjustDamageTakenScaleServerRpc(playerController.defaultDamageTakenScale); // Reset Damage Taken
            playersToStun.Remove(playerController);
            Debug.Log("EXIT COUNT: " + playersToStun.Count);
            Debug.Log("EXIT TEAMID: " + teamId.Value);

        }
        exitAdjustMoveSpeed = false;
    }
    private void ApplyStunToPlayers()
    {
        foreach (PlayerController playerController in playersToStun)
        {
            if (playerController != null)
            {
                playerController.RequestStunServerRpc(stunDuration);
                playerController.AdjustMovementSpeedServerRpc(playerController.defaultMoveSpeed); // Reset MoveSpeed
                playerController.AdjustDamageTakenScaleServerRpc(playerController.defaultDamageTakenScale); // Reset Damage Taken
            }
        }
        playersToStun.Clear(); // Clear the list after applying stun
    }
}