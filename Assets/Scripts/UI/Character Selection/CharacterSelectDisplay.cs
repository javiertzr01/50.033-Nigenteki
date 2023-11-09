using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;


public class CharacterSelectDisplay : NetworkBehaviour
{
    [SerializeField] private GameObject characterInfoPanel;
    [SerializeField] private TMP_Text characterNameText; 

    private NetworkList<CharacterSelectState> players;
    public void Awake()
    {
        players = new NetworkList<CharacterSelectState>();
    }
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;

            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            {
                HandleClientConnected(client.ClientId);
            }
        }
    }

    private void HandleClientConnected(ulong clientId)
    {
        players.Add(new CharacterSelectState(clientId));
    }

    private void HandleClientDisconnected(ulong clientId)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientId == clientId) ;
            {
                players.RemoveAt(i);
                break;
            }
        }
    }

    public void Select(BuildPlayerVariables player)
    {
        characterNameText.text = player.DisplayName;

        characterInfoPanel.SetActive(true);

        SelectServerRpc(player.Id, player.LeftArmId, player.RightArmId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SelectServerRpc(int characterId, int leftArmId, int rightArmId, ServerRpcParams serverRpcParams = default)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientId == serverRpcParams.Receive.SenderClientId)
            {
                players[i] = new CharacterSelectState(
                    players[i].ClientId,
                    characterId,
                    leftArmId,
                    rightArmId
                );
            }
        }
    }
}
