using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeftArmSelectButton : MonoBehaviour
{
    [SerializeField] private Image iconImage;

    private CharacterSelectDisplay characterSelect;

    private BuildArmVariables leftArm;

    public void SetLeftArm(CharacterSelectDisplay characterSelect, BuildArmVariables leftArm)
    {
        iconImage.sprite = leftArm.Icon;

        this.characterSelect = characterSelect;
        this.leftArm = leftArm;
    }

    public void SelectLeftArm()
    {
        characterSelect.SelectLeftArmDisplay(leftArm);
    }
}
