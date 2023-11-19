using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode; 

public class SynchronizeNetworkVariables : NetworkBehaviour
{
    public NetworkStore netStore;
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        }
        else
        {
            
        }
    }

    private void NetworkManager_OnClientConnectedCallback(ulong obj)
    {
        
    }

    private void OnGeneratedMapDataChanged()
    {
        
    }

    private void StartChangingNetworkVariable()
    {
        netStore.generatedMapData.Value = netStore.generatedMapData.Value;

        NetworkManager.Singleton.OnClientConnectedCallback -= NetworkManager_OnClientConnectedCallback;
    }
}
