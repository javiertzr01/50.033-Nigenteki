using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RightArmSelectButton : MonoBehaviour
{
    [SerializeField] private Image iconImage;

    private CharacterSelectDisplay characterSelect;

    private BuildArmVariables rightArm;

    public void SetRightArm(CharacterSelectDisplay characterSelect, BuildArmVariables rightArm)
    {
        iconImage.sprite = rightArm.Icon;

        this.characterSelect = characterSelect;
        this.rightArm = rightArm;
    }

    public void SelectRightArm()
    {
        characterSelect.SelectRightArmDisplay(rightArm);
    }
}
