using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private CharacterDatabaseVariables characterDatabase;
    [SerializeField] private ArmDatabaseVariables armDatabase;
    [SerializeField] private GameObject leftArmHolderPrefab;
    [SerializeField] private GameObject rightArmHolderPrefab;

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
                var spawnPos = new Vector2(Random.Range(-3f, 3f), Random.Range(-3f, 3f));
                var characterInstance = Instantiate(character.CharacterPrefab, spawnPos, Quaternion.identity);
                characterInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(client.Value.clientId);
                characterInstance.GetComponent<NetworkObject>().ChangeOwnership(client.Value.clientId);
                characterInstance.GetComponent<PlayerController>().leftArmPrefab = leftArm.ArmPrefab;
                characterInstance.GetComponent<PlayerController>().rightArmPrefab = rightArm.ArmPrefab;

               /* GameObject leftArmHolderClone = Instantiate(leftArmHolderPrefab, characterInstance.transform.GetComponent<NetworkObject>().transform.position + leftArmHolderPrefab.transform.localPosition, Quaternion.Euler(0, 0, 0));
                //leftArmHolderClone.transform.GetComponent<NetworkObject>().Spawn(true);
                leftArmHolderClone.transform.GetComponent<NetworkObject>().SpawnWithOwnership(client.Value.clientId);
                leftArmHolderClone.GetComponent<NetworkObject>().TrySetParent(characterInstance.transform);
                //leftArmHolder = leftArmHolderClone;

                GameObject rightArmHolderClone = Instantiate(rightArmHolderPrefab, characterInstance.transform.GetComponent<NetworkObject>().transform.position + rightArmHolderPrefab.transform.localPosition, Quaternion.Euler(0, 0, 0));
                //rightArmHolderClone.transform.GetComponent<NetworkObject>().Spawn(true);
                rightArmHolderClone.transform.GetComponent<NetworkObject>().SpawnWithOwnership(client.Value.clientId);
                rightArmHolderClone.GetComponent<NetworkObject>().TrySetParent(characterInstance.transform);
                //rightArmHolder = rightArmHolderClone;

                GameObject leftArmClone = Instantiate(leftArm.ArmPrefab, leftArmHolderClone.transform);
                //leftArmClone.transform.GetComponent<NetworkObject>().Spawn(true);
                leftArmClone.transform.GetComponent<NetworkObject>().SpawnWithOwnership(client.Value.clientId);
                leftArmClone.transform.GetComponent<NetworkObject>().TrySetParent(leftArmHolderClone.transform);


                GameObject rightArmClone = Instantiate(rightArm.ArmPrefab, rightArmHolderClone.transform);
                //rightArmClone.transform.GetComponent<NetworkObject>().Spawn(true);
                rightArmClone.transform.GetComponent<NetworkObject>().SpawnWithOwnership(client.Value.clientId);
                rightArmClone.transform.GetComponent<NetworkObject>().TrySetParent(rightArmHolderClone.transform);*/
            }

        }
    }
}
