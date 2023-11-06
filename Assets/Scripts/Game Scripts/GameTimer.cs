using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameTimer", menuName = "ScriptableObjects/GameTimer", order = 1)]
public class GameTimer : ScriptableObject
{
    public int PhaseNumber;
    public int CurrentSecondsLeft;
}
