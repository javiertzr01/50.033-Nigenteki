using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReadyButton : MonoBehaviour
{
    private CharacterSelectDisplay characterSelect;

    private bool isReady;

    public void SetReadyState(CharacterSelectDisplay characterSelect)
    {
        this.characterSelect = characterSelect;
    }

    public void SetReadyStateDisplay()
    {
        if (isReady)
            isReady = false;
        else
            isReady = true;

        characterSelect.PlayerReadyDisplay(isReady);
    }
}
