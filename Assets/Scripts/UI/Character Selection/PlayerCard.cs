using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCard : MonoBehaviour
{
    [SerializeField] private CharacterDatabaseVariables characterDatabase;
    [SerializeField] private ArmDatabaseVariables armDatabase;

    [SerializeField] private GameObject playerInfo;
    [SerializeField] private Image characterIconImage;
    [SerializeField] private Image leftArmIconImage;
    [SerializeField] private Image rightArmIconImage;

    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text characterNameText;
    [SerializeField] private TMP_Text playerReadyText;

    public void UpdateCharacterDisplay(CharacterSelectState characterState)
    {
        if (characterState.CharacterId != -1)
        {
            var character = characterDatabase.GetCharacterById(characterState.CharacterId);
            characterIconImage.sprite = character.Icon;
            characterIconImage.enabled = true;
            characterNameText.text = character.DisplayName;
        }
        else
        {
            characterIconImage.enabled = false;
        }

        playerNameText.text = $"Player {characterState.ClientId}";

        playerInfo.SetActive(true);
    }

    public void UpdateLeftArmDisplay(ArmSelectState leftArmState)
    {
        if (leftArmState.ArmId != -1)
        {
            var leftArm = armDatabase.GetArmById(leftArmState.ArmId);
            leftArmIconImage.sprite = leftArm.Icon;
            leftArmIconImage.enabled = true;
        }
        else
        {
            leftArmIconImage.enabled = false;
        }
    }

    public void UpdateRightArmDisplay(ArmSelectState rightArmState)
    {
        if (rightArmState.ArmId != -1)
        {
            var rightArm = armDatabase.GetArmById(rightArmState.ArmId);
            rightArmIconImage.sprite = rightArm.Icon;
            rightArmIconImage.enabled = true;
        }
        else
        {
            rightArmIconImage.enabled = false;
        }
    }

    public void UpdatePlayerReadyDisplay(PlayerReadyState playerReadyState)
    {
        if (playerReadyState.IsReady)
        {
            playerReadyText.text = "Ready!";
        }
        else
        {
            playerReadyText.text = "Not Ready";
        }
    }

    public void DisableDisplay()
    {
        playerInfo.SetActive(false);
    }
}
