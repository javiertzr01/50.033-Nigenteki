using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;

public class StartClientButton : MonoBehaviour
{
    public UnityEvent startJoinRelay;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartClient()
    {
        startJoinRelay.Invoke();

        if (NetworkManager.Singleton.StartClient())
            Debug.Log("Started Client");
    }
}
