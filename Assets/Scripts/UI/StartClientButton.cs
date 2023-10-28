using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class StartClientButton : MonoBehaviour
{

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
        if (NetworkManager.Singleton.StartClient())
            Debug.Log("Started Client");
    }
}
