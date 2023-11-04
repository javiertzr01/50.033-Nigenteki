using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Unity.Netcode;
using TMPro;

public class UIManager : NetworkBehaviour
{
    public NetworkStore netStore;

    public UnityEvent generateMap;
    public UnityEvent loadMap;
    public UnityEvent saveMap;

    [SerializeField]
    private Button startHostButton;

    [SerializeField]
    private Button startServerButton;

    [SerializeField]
    private Button startClientButton;

    [SerializeField]
    private TextMeshProUGUI playersInGameText;

    [SerializeField]
    private TMP_InputField joinCodeInputField;

    private void Awake()
    {
        Cursor.visible = true;
    }

    // Start is called before the first frame update
    private void Start()
    {

        startServerButton?.onClick.AddListener(() =>
        {
            if (NetworkManager.Singleton.StartServer())
            {
                Logger.Instance.LogInfo("Server Started");
            }
        });

        startHostButton?.onClick.AddListener(async () =>
        {
            if (RelayManager.Instance.IsRelayEnabled)
                await RelayManager.Instance.SetupRelay();

            if (NetworkManager.Singleton.StartHost())
                Logger.Instance.LogInfo("Host Started");

            // Generate random map
            generateMap.Invoke();
            Logger.Instance.LogInfo("Random map generated");
            saveMap.Invoke();
            Logger.Instance.LogInfo("Random map instance saved");

        });

        startClientButton?.onClick.AddListener(async () =>
        {
            if (RelayManager.Instance.IsRelayEnabled && !string.IsNullOrEmpty(joinCodeInputField.text))
                await RelayManager.Instance.JoinRelay(joinCodeInputField.text);

            if (NetworkManager.Singleton.StartClient())
                Logger.Instance.LogInfo("Client Started");

        });
    }

    // Update is called once per frame
    private void Update()
    {
        //playersInGameText.text = $"Players in game: {LobbyManager.Instance.playersInGame}";
    }

    public void UpdatePlayersInLobbyText()
    {
        playersInGameText.text = $"Players in game: {netStore.playersInLobby.Value}";
    }
}
