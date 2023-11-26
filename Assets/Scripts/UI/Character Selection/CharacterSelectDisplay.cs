using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectDisplay : NetworkBehaviour
{
    [SerializeField] private CharacterDatabaseVariables characterDatabase;
    [SerializeField] private ArmDatabaseVariables armDatabase;

    [SerializeField] private Transform charactersHolder;
    [SerializeField] private Transform leftArmsHolder;
    [SerializeField] private Transform rightArmsHolder;

    [SerializeField] private CharacterSelectButton selectCharacterButtonPrefab;
    [SerializeField] private LeftArmSelectButton selectLeftArmButtonPrefab;
    [SerializeField] private RightArmSelectButton selectRightArmButtonPrefab;

    [SerializeField] private PlayerCard[] playerCards;

    [SerializeField] private GameObject characterInfoPanel;
    [SerializeField] private TMP_Text characterNameText;

    [SerializeField] private GameObject leftArmInfoPanel;
    [SerializeField] private TMP_Text leftArmNameText;

    [SerializeField] private GameObject rightArmInfoPanel;
    [SerializeField] private TMP_Text rightArmNameText;

    [SerializeField] private TMP_Text hostJoinCodeText;

    [SerializeField] private Button readyButton;

    private NetworkList<CharacterSelectState> players;
    private NetworkList<ArmSelectState> leftArms;
    private NetworkList<ArmSelectState> rightArms;
    private NetworkList<PlayerReadyState> playersReady;

    public void Awake()
    {
        players = new NetworkList<CharacterSelectState>();
        leftArms = new NetworkList<ArmSelectState>();
        rightArms = new NetworkList<ArmSelectState>();
        playersReady = new NetworkList<PlayerReadyState>();
    }
    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            BuildCharacterVariables[] allCharacters = characterDatabase.GetAllCharacters();
            BuildArmVariables[] allArms = armDatabase.GetAllArms();

            foreach (var character in allCharacters)
            {
                var selectButtonInstance = Instantiate(selectCharacterButtonPrefab, charactersHolder);
                selectButtonInstance.SetCharacter(this, character);
            }

            foreach (var leftArm in allArms)
            {
                var selectButtonInstance = Instantiate(selectLeftArmButtonPrefab, leftArmsHolder);
                selectButtonInstance.SetLeftArm(this, leftArm);
            }

            foreach (var rightArm in allArms)
            {
                var selectButtonInstance = Instantiate(selectRightArmButtonPrefab, rightArmsHolder);
                selectButtonInstance.SetRightArm(this, rightArm);
            }

            readyButton.GetComponent<PlayerReadyButton>().SetReadyState(this);

            players.OnListChanged += HandlePlayersStateChanged;
            leftArms.OnListChanged += HandleLeftArmStateChanged;
            rightArms.OnListChanged += HandleRightArmStateChanged;
            playersReady.OnListChanged += HandlePlayersReadyStateChanged;

            hostJoinCodeText.text = RelayManager.Instance.joinCode;
        }

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;

            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            {
                HandleClientConnected(client.ClientId);
            }

            hostJoinCodeText.text = RelayManager.Instance.joinCode;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            players.OnListChanged += HandlePlayersStateChanged;
        }

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
        }
    }

    private void HandleClientConnected(ulong clientId)
    {
        players.Add(new CharacterSelectState(clientId));
        leftArms.Add(new ArmSelectState(clientId));
        rightArms.Add(new ArmSelectState(clientId));
        playersReady.Add(new PlayerReadyState(clientId));
    }

    private void HandleClientDisconnected(ulong clientId)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientId == clientId)
            {
                players.RemoveAt(i);
                break;
            }
        }
    }

    private void HandlePlayersStateChanged(NetworkListEvent<CharacterSelectState> changeEvent)
    {
        for (int i = 0; i < playerCards.Length; i++)
        {
            if (players.Count > i)
            {
                playerCards[i].UpdateCharacterDisplay(players[i]);
            }
            else
            {
                playerCards[i].DisableDisplay();
            }
        }
    }

    private void HandleLeftArmStateChanged(NetworkListEvent<ArmSelectState> changeEvent)
    {
        for (int i = 0; i < playerCards.Length; i++)
        {
            if (players.Count > i)
            {
                playerCards[i].UpdateLeftArmDisplay(leftArms[i]);
            }
            else
            {
                playerCards[i].DisableDisplay();
            }
        }
    }
    private void HandleRightArmStateChanged(NetworkListEvent<ArmSelectState> changeEvent)
    {
        for (int i = 0; i < playerCards.Length; i++)
        {
            if (players.Count > i)
            {
                playerCards[i].UpdateRightArmDisplay(rightArms[i]);
            }
            else
            {
                playerCards[i].DisableDisplay();
            }
        }
    }

    private void HandlePlayersReadyStateChanged(NetworkListEvent<PlayerReadyState> changeEvent)
    {
        for (int i = 0; i < playerCards.Length; i++)
        {
            if (players.Count > i)
            {
                playerCards[i].UpdatePlayerReadyDisplay(playersReady[i]);
            }
            else
            {
                playerCards[i].DisableDisplay();
            }
        }
    }

    public void SelectCharacterDisplay(BuildCharacterVariables character)
    {
        characterNameText.text = character.DisplayName;

        characterInfoPanel.SetActive(true);

        SelectCharacterDisplayServerRpc(character.Id);
    } 

    [ServerRpc(RequireOwnership = false)]
    private void SelectCharacterDisplayServerRpc(int characterId, ServerRpcParams serverRpcParams = default)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientId == serverRpcParams.Receive.SenderClientId)
            {
                players[i] = new CharacterSelectState(
                    players[i].ClientId,
                    characterId
                );
            }
        }
    }

    public void SelectLeftArmDisplay(BuildArmVariables leftArm)
    {
        leftArmNameText.text = leftArm.DisplayName;

        leftArmInfoPanel.SetActive(true);

        SelectLeftArmDisplayServerRpc(leftArm.Id);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SelectLeftArmDisplayServerRpc(int leftArmId, ServerRpcParams serverRpcParams = default)
    {
        for (int i = 0; i < leftArms.Count; i++)
        {
            if (leftArms[i].ClientId == serverRpcParams.Receive.SenderClientId)
            {
                leftArms[i] = new ArmSelectState(
                    leftArms[i].ClientId,
                    leftArmId
                );
            }
        }
    }

    public void SelectRightArmDisplay(BuildArmVariables rightArm)
    {
        rightArmNameText.text = rightArm.DisplayName;

        rightArmInfoPanel.SetActive(true);

        SelectRightArmServerRpc(rightArm.Id);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SelectRightArmServerRpc(int rightArmId, ServerRpcParams serverRpcParams = default)
    {
        for (int i = 0; i < rightArms.Count; i++)
        {
            if (rightArms[i].ClientId == serverRpcParams.Receive.SenderClientId)
            {
                rightArms[i] = new ArmSelectState(
                    rightArms[i].ClientId,
                    rightArmId
                );
            }
        }
    }

    public void PlayerReadyDisplay(bool isReady)
    {
        PlayerReadyServerRpc(isReady);
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayerReadyServerRpc(bool isReady, ServerRpcParams serverRpcParams = default)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (playersReady[i].ClientId == serverRpcParams.Receive.SenderClientId)
            {
                playersReady[i] = new PlayerReadyState(
                        playersReady[i].ClientId,
                        isReady
                    );
            }
        }

        foreach (var playerReady in playersReady)
        {
            if (!playerReady.IsReady) return;
        }

        for (int i = 0; i < playersReady.Count; i++)
        {
            ServerManager.Instance.SetPlayer(playersReady[i].ClientId, players[i].CharacterId, leftArms[i].ArmId, rightArms[i].ArmId);
        }

        ServerManager.Instance.StartGame();
    }
}
