using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;

public class HoneyComb : SkillObject
{
    private float countdownTimer = 5f; // Countdown timer
    public float healingValue = 10;
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
    }

    public override void TriggerEnter2DLogic(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            instantiatingArm.ChargeUltimate(healingValue, 50);
            // Friendly Player here
            // TODO: Implement a list within the PlayerController that stores the unique healing abilities on the player

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
        // If friendly player
        // Check if the HoneybeeSpray is within the HealStore list, if not then heal player

    }

    public override void TriggerExit2DLogic(Collider2D other)
    {
        // Friendly Player
        // Remove "Honeybee Spray" from Player HealStore list


        // // Enemy Player here
        // PlayerController playerController = other.GetComponent<PlayerController>();
        // if (playerController != null && enemiestoSlow.Contains(playerController))
        // {
        //     playerController.MoveSpeed *= 2f; // Reset MoveSpeed
        //     enemiestoSlow.Remove(playerController);
        // }
    }


    public override void CollisionEnter2DLogic(Collision2D collider)
    {
        // throw new System.NotImplementedException();
    }


}