using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;

public class StartHostButton : MonoBehaviour
{

    public UnityEvent startRelaySetup;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartHost()
    {
        startRelaySetup.Invoke();

        if (NetworkManager.Singleton.StartHost())
            Debug.Log("Started Host");
    }
}
