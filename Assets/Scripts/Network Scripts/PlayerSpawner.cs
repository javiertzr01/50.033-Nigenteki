using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;

public class PlayerSpawner : NetworkBehaviour
{
    public NetworkStore netStore;
    private List<ulong> redIds = new List<ulong>();
    private List<ulong> blueIds = new List<ulong>();

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
                characterInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(client.Value.clientId, true);
                characterInstance.GetComponent<NetworkObject>().ChangeOwnership(client.Value.clientId);
                characterInstance.GetComponent<PlayerController>().teamId.Value = teamId;
                characterInstance.GetComponent<PlayerController>().leftArmPrefab = leftArm.ArmPrefab;
                characterInstance.GetComponent<PlayerController>().rightArmPrefab = rightArm.ArmPrefab;

                if (teamId == 0)
                {
                    redIds.Add(client.Value.clientId);
                }
                if (teamId == 1)
                {
                    blueIds.Add(client.Value.clientId);
                }
            }

        }
    }

    public (List<ulong>, List<ulong>) getTeamIds()
    {
        return (blueIds, redIds);
    }
}
