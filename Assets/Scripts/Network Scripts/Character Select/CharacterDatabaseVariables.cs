using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDatabaseVariables", menuName = "NetworkScriptableObjects/CharacterDatabaseVariables", order = 8)]
public class CharacterDatabaseVariables : ScriptableObject
{
    [SerializeField] private BuildCharacterVariables[] characters = new BuildCharacterVariables[0];

    public BuildCharacterVariables[] GetAllCharacters() => characters;

    public BuildCharacterVariables GetCharacterById(int id)
    {
        foreach (var character in characters)
        {
            if (character.Id == id)
            {
                return character;
            }
        }

        return null;
    }
}
