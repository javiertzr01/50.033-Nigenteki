using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectButton : MonoBehaviour
{
    [SerializeField] private Image iconImage;

    private CharacterSelectDisplay characterSelect;

    private BuildPlayerVariables player;

    public void SetCharacter(CharacterSelectDisplay characterSelect, BuildPlayerVariables player)
    {
        this.characterSelect = characterSelect;
        this.player = player;
    }

    public void SelectCharacter()
    {
        characterSelect.Select(player);
    }


}
