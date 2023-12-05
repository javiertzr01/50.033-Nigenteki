using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class ServerManager : Singleton<ServerManager>
{
    [SerializeField] private string characterSelectScreenSceneName = "CharacterSelect";
    [SerializeField] private string gameplaySceneName = "MultiplayerTest";

    private bool gameStarted;
    public Dictionary<ulong, ClientData> ClientData { get; private set; }

    public async void StartServer()
    {
        if (RelayManager.Instance.IsRelayEnabled)
            await RelayManager.Instance.SetupRelay();

        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.OnServerStarted += OnNetworkReady;

        ClientData = new Dictionary<ulong, ClientData>();

        NetworkManager.Singleton.StartServer();
    }

    public async void StartHost()
    {
        if (RelayManager.Instance.IsRelayEnabled)
            await RelayManager.Instance.SetupRelay();

        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.OnServerStarted += OnNetworkReady;

        ClientData = new Dictionary<ulong, ClientData>();

        // if (NetworkManager.Singleton.StartHost())
            //Logger.Instance.LogInfo("Host Started");
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        if (ClientData.Count >= 6 || gameStarted)
        {
            response.Approved = false;
            return;
        }

        response.Approved = true;
        response.CreatePlayerObject = false;
        response.Pending = false;

        ClientData[request.ClientNetworkId] = new ClientData(request.ClientNetworkId);

        //Logger.Instance.LogInfo($"Added client {request.ClientNetworkId}"); 
    }

    private void OnNetworkReady()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;

        NetworkManager.Singleton.SceneManager.LoadScene(characterSelectScreenSceneName, LoadSceneMode.Single);
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if (ClientData.ContainsKey(clientId))
        {
            if (ClientData.Remove(clientId))
            {
                //Logger.Instance.LogInfo($"Removed client {clientId}");
            }
        }
    }

    public void SetPlayer(ulong clientID, int characterId, int teamId, int leftArmId, int rightArmId)
    {
        if (ClientData.TryGetValue(clientID, out ClientData data))
        {
            data.characterId = characterId;
            data.teamId = teamId;
            data.leftArmId = leftArmId;
            data.rightArmId = rightArmId;
        }

        //Logger.Instance.LogInfo($"Set Player {clientID}'s character as team {teamId}, character as {characterId}, left arm as {leftArmId}, right arm as {rightArmId}");
    }

    public void StartGame()
    {
        gameStarted = true;

        NetworkManager.Singleton.SceneManager.LoadScene(gameplaySceneName, LoadSceneMode.Single);
    }
}
