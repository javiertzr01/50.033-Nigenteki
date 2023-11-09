using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDatabaseVariables", menuName = "NetworkScriptableObjects/PlayerDatabaseVariables", order = 8)]
public class PlayerDatabaseVariables : ScriptableObject
{
    [SerializeField] private BuildPlayerVariables[] players = new BuildPlayerVariables[0];

    public BuildPlayerVariables[] GetAllPlayers => players;

    public BuildPlayerVariables GetPlayerById(int id)
    {
        foreach (var player in players)
        {
            if (player.Id == id)
            {
                return player;
            }
        }

        return null;
    }
}
