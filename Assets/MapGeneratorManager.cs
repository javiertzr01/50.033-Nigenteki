using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class MapGeneratorManager : NetworkBehaviour
{
    public UnityEvent generateMap;
    public UnityEvent saveMap;
    public UnityEvent loadMap;
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            generateMap.Invoke();
            //Logger.Instance.LogInfo("Generated Map");
            saveMap.Invoke();
            //Logger.Instance.LogInfo("Saving Map");
        }
    }

    public void Start()
    {
        if (!IsServer && IsClient)
        {
            loadMap.Invoke();
        }

    }
}
