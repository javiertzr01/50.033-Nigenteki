using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using TMPro;

public class RelayManager : Singleton<RelayManager>
{
    [SerializeField]
    private string environment = "production";

    [SerializeField]
    private int maxConnections = 10;

    public bool IsRelayEnabled => Transport != null && 
        Transport.Protocol == UnityTransport.ProtocolType.RelayUnityTransport;

    public UnityTransport Transport => NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();

    public TMP_InputField joinCodeInputField;

    public async Task<RelayHostData> SetupRelay()
    {
        InitializationOptions options = new InitializationOptions()
            .SetEnvironmentName(environment);

        await UnityServices.InitializeAsync(options);

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        Allocation allocation = await Relay.Instance.CreateAllocationAsync(maxConnections);

        RelayHostData relayHostData = new RelayHostData
        {
            Key = allocation.Key,
            Port = (ushort)allocation.RelayServer.Port,
            AllocationID = allocation.AllocationId,
            AllocationIDBytes = allocation.AllocationIdBytes,
            IPv4Address = allocation.RelayServer.IpV4,
            ConnectionData = allocation.ConnectionData
        };

        relayHostData.JoinCode = await Relay.Instance.GetJoinCodeAsync(relayHostData.AllocationID);

        Transport.SetRelayServerData(relayHostData.IPv4Address, relayHostData.Port, relayHostData.AllocationIDBytes,
            relayHostData.Key, relayHostData.ConnectionData);

        Logger.Instance.LogInfo($"Relay Server generated a join code {relayHostData.JoinCode}");

        return relayHostData;
    }

    public async Task<RelayJoinData> JoinRelay(string joinCode)
    {
        InitializationOptions options = new InitializationOptions()
            .SetEnvironmentName(environment);

        await UnityServices.InitializeAsync(options);

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        JoinAllocation allocation = await Relay.Instance.JoinAllocationAsync(joinCode);

        RelayJoinData relayJoinData = new RelayJoinData
        {
            Key = allocation.Key,
            Port = (ushort)allocation.RelayServer.Port,
            AllocationID = allocation.AllocationId,
            AllocationIDBytes = allocation.AllocationIdBytes,
            ConnectionData = allocation.ConnectionData,
            HostConnectionData = allocation.HostConnectionData,
            IPv4Address = allocation.RelayServer.IpV4,
            JoinCode = joinCode
        };

        Transport.SetRelayServerData(relayJoinData.IPv4Address, relayJoinData.Port, relayJoinData.AllocationIDBytes,
            relayJoinData.Key, relayJoinData.ConnectionData, relayJoinData.HostConnectionData);

        Logger.Instance.LogInfo($"Client joined game with join code {joinCode}");

        return relayJoinData;
    }
}
