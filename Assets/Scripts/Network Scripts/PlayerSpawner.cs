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
            var leftArm = armDatabase.GetArmById(client.Value.leftArmId);
            var rightArm = armDatabase.GetArmById(client.Value.rightArmId);

            if (character != null && leftArm != null && rightArm != null)
            {
                // Player spawning can be done here
                //var spawnPos = new Vector2(Random.Range(-3f, 3f), Random.Range(-3f, 3f));
                Vector2 spawnPos = netStore.generatedMapData.Value.RedSpawnPosition;
                var characterInstance = Instantiate(character.CharacterPrefab, spawnPos, Quaternion.identity);
                characterInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(client.Value.clientId);
                characterInstance.GetComponent<NetworkObject>().ChangeOwnership(client.Value.clientId);
                characterInstance.GetComponent<PlayerController>().leftArmPrefab = leftArm.ArmPrefab;
                characterInstance.GetComponent<PlayerController>().rightArmPrefab = rightArm.ArmPrefab;
            }

        }
    }
}
