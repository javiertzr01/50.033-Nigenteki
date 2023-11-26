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
                instantiatingArm.ChargeUltimate(healingValue, 5);

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

    public override void TriggerEnter2DLogic(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            // TODO: Team Separation

            // Friendly Player here
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null && !playerController.interactingWithHoneyComb && !alliesToHeal.Contains(playerController))
            {
                alliesToHeal.Add(playerController);
                playerController.interactingWithHoneyComb = true;
                Debug.Log("HoneyComb Trigger Enter: " + playerController.interactingWithHoneyComb);
                playerController.healingPerSecond += healingValue;
            }


            // // Enemy Player here
            // PlayerController playerController = other.GetComponent<PlayerController>();
            // if (playerController != null && !enemiestoSlow.Contains(playerController))
            // {
            //     playerController.MoveSpeed /= 2f; // Halve MoveSpeed
            //     enemiestoSlow.Add(playerController);
            // }

        }

    }

    public override void TriggerStay2DLogic(Collider2D other)
    {
    }

    public override void TriggerExit2DLogic(Collider2D other)
    {
        // TODO: Team Separation

        // Friendly Player
        PlayerController playerController = other.GetComponent<PlayerController>();
        if (playerController != null && playerController.interactingWithHoneyComb && alliesToHeal.Contains(playerController))
        {
            alliesToHeal.Remove(playerController);
            playerController.interactingWithHoneyComb = false;
            Debug.Log("HoneyComb Trigger Enter: " + playerController.interactingWithHoneyComb);
            playerController.healingPerSecond -= healingValue;
        }


        // // Enemy Player here
        // PlayerController playerController = other.GetComponent<PlayerController>();
        // if (playerController != null && enemiestoSlow.Contains(playerController))
        // {
        //     playerController.MoveSpeed *= 2f; // Reset MoveSpeed
        //     enemiestoSlow.Remove(playerController);
        // }
    }


}