using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectButton : MonoBehaviour
{
    [SerializeField] private Image iconImage;

    private CharacterSelectDisplay characterSelect;

    private BuildCharacterVariables character;


    public void SetCharacter(CharacterSelectDisplay characterSelect, BuildCharacterVariables character)
    {
        iconImage.sprite = character.Icon;

        this.characterSelect = characterSelect;
        this.character = character;
    }

    public void SelectCharacter()
    {
        characterSelect.SelectCharacterDisplay(character);
    }

}
