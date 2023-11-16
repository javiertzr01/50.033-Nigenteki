using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class GameLobby : MonoBehaviour
{
    // Start is called before the first frame update
    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Logger.Instance.LogError("Signed in " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private async void CreateLobby()
    {
        try
        {
            string lobbyName = "Game Lobby";
            int maxPlayers = 6;

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers);

            Logger.Instance.LogInfo($"Lobby created with name {lobby.Name} and size {lobby.MaxPlayers}");
        }
        catch (LobbyServiceException e)
        {
            Logger.Instance.LogError(e.ToString());
        }
    }

    private async void ListLobbies()
    {
        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            Logger.Instance.LogInfo($"Lobbies found: " + queryResponse.Results.Count);
            foreach (Lobby lobby in queryResponse.Results)
            {
                Logger.Instance.LogInfo($"Lobby {lobby.Name} with {lobby.MaxPlayers}");
            }
        }
        catch (LobbyServiceException e)
        {
            Logger.Instance.LogInfo(e.ToString());
        }
    }
}
