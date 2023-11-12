using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class ServerManager : Singleton<ServerManager>
{
    [SerializeField] private string characterSelectScreenSceneName = "CharacterSelect";

    private bool gameStarted;
    public Dictionary<ulong, ClientData> ClientData { get; private set; }

    public void StartServer()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.OnServerStarted += OnNetworkReady;

        NetworkManager.Singleton.StartServer();
    }

    public async void StartHost()
    {
        if (RelayManager.Instance.IsRelayEnabled)
            await RelayManager.Instance.SetupRelay();

        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.OnServerStarted += OnNetworkReady;

        if (NetworkManager.Singleton.StartHost())
            Logger.Instance.LogInfo("Host Started");
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
    }

    private void OnNetworkReady()
    {
        NetworkManager.Singleton.SceneManager.LoadScene(characterSelectScreenSceneName, LoadSceneMode.Single);
    }
}
