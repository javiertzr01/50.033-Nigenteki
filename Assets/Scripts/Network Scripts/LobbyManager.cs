using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;

public class LobbyManager : NetworkBehaviour
{

    public LobbyVariables lobbyVariables;

    public UnityEvent updatePlayersInLobbyUI;

    public int PlayersInLobby
    {
        get { return lobbyVariables.playersInLobby.Value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            if (IsServer)
            {
                lobbyVariables.playersInLobby.Value++;

                updatePlayersInLobbyUI.Invoke();

                Debug.Log($"Player {id} just joined the lobby");
                Debug.Log($"Current number of players in lobby: {lobbyVariables.playersInLobby.Value}");
            }
        };

        NetworkManager.Singleton.OnClientDisconnectCallback += (id) =>
        {
            if (IsServer)
            {
                lobbyVariables.playersInLobby.Value--;

                updatePlayersInLobbyUI.Invoke();

                Debug.Log($"Player {id} just left the lobby");
                Debug.Log($"Current number of players in lobby: {lobbyVariables.playersInLobby.Value}");
            }
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
