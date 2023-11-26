using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerSpawner : NetworkBehaviour
{
    public NetworkStore netStore;

    [SerializeField] private CharacterDatabaseVariables characterDatabase;
    [SerializeField] private ArmDatabaseVariables armDatabase;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        foreach (var client in ServerManager.Instance.ClientData)
        {
            var character = characterDatabase.GetCharacterById(client.Value.characterId);
            var teamId = character.TeamId;
            var leftArm = armDatabase.GetArmById(client.Value.leftArmId);
            var rightArm = armDatabase.GetArmById(client.Value.rightArmId);

            if (character != null && leftArm != null && rightArm != null)
            {
                Vector2 spawnPos = new Vector2(0,0);
                if (teamId == 0)
                {
                    spawnPos = netStore.generatedMapData.Value.RedSpawnPosition;
                }
                else if (teamId == 1)
                {
                    spawnPos = netStore.generatedMapData.Value.BlueSpawnPosition;
                }
                var characterInstance = Instantiate(character.CharacterPrefab, spawnPos, Quaternion.identity);
                characterInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(client.Value.clientId);
                characterInstance.GetComponent<NetworkObject>().ChangeOwnership(client.Value.clientId);
                characterInstance.GetComponent<PlayerController>().leftArmPrefab = leftArm.ArmPrefab;
                characterInstance.GetComponent<PlayerController>().rightArmPrefab = rightArm.ArmPrefab;
            }

        }
    }
}
