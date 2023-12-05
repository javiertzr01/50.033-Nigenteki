using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class MainMenuDisplay : MonoBehaviour
{


    [SerializeField] private TMP_InputField joinCodeInputField;


    public async void StartHost()
    {
        /*if (RelayManager.Instance.IsRelayEnabled)
            await RelayManager.Instance.SetupRelay();

        if (NetworkManager.Singleton.StartHost())
            //Logger.Instance.LogInfo("Host Started");

        NetworkManager.Singleton.SceneManager.LoadScene(characterSelectScreenSceneName, LoadSceneMode.Single);*/

        ServerManager.Instance.StartHost();
    }

    public async void StartServer()
    {
        /*NetworkManager.Singleton.StartServer();
        NetworkManager.Singleton.SceneManager.LoadScene(characterSelectScreenSceneName, LoadSceneMode.Single);*/
        ServerManager.Instance.StartServer();
    }

    public async void StartClient()
    {
        if (RelayManager.Instance.IsRelayEnabled && !string.IsNullOrEmpty(joinCodeInputField.text))
            await RelayManager.Instance.JoinRelay(joinCodeInputField.text);

        // if (NetworkManager.Singleton.StartClient())
            //Logger.Instance.LogInfo("Client Started");
    }
}
