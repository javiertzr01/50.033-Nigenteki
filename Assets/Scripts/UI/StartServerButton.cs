using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class StartServerButton : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartServer()
    {
        if (NetworkManager.Singleton.StartServer())
            Debug.Log("Started Server");
    }
}
