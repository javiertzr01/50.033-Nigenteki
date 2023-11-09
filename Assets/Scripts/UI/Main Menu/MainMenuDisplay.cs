using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class MainMenuDisplay : MonoBehaviour
{
    [SerializeField] private string characterSelectScreenSceneName = "CharacterSelect";

    public async void StartHost()
    {
        if (RelayManager.Instance.IsRelayEnabled)
            await RelayManager.Instance.SetupRelay();

        if (NetworkManager.Singleton.StartHost())
            Logger.Instance.LogInfo("Host Started");

        NetworkManager.Singleton.SceneManager.LoadScene(characterSelectScreenSceneName, LoadSceneMode.Single);
    }

    public async void StartServer()
    {
        NetworkManager.Singleton.StartServer();
        NetworkManager.Singleton.SceneManager.LoadScene(characterSelectScreenSceneName, LoadSceneMode.Single);
    }

    public async void StartClient()
    {
        //if (RelayManager.Instance.IsRelayEnabled)
            //await RelayManager.Instance.JoinRelay();

        if (NetworkManager.Singleton.StartClient())
            Logger.Instance.LogInfo("Client Started");
    }
}
