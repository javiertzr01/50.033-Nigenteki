using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;

public class HoneyComb : SkillObject
{
    [System.NonSerialized] public float countdownTimer; // Countdown timer
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
    }

    public override void TriggerEnter2DLogic(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            // instantiatingArm.ChargeUltimate(healingValue, 50);

            // Friendly Player here
            // TODO: Implement a list within the PlayerController that stores the unique healing abilities on the player
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null && !playerController.interactingWithHoneyComb)
            {
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
        // Adjust per second healing percentage
    }

    public override void TriggerExit2DLogic(Collider2D other)
    {
        // Friendly Player
        PlayerController playerController = other.GetComponent<PlayerController>();
        if (playerController != null && playerController.interactingWithHoneyComb)
        {
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


    public override void CollisionEnter2DLogic(Collision2D collider)
    {
        // throw new System.NotImplementedException();
    }


}