using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class StartHostButton : MonoBehaviour
{

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
        if (NetworkManager.Singleton.StartHost())
            Debug.Log("Started Host");
    }
}
