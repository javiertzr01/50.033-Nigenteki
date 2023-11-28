using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Crystal : NetworkBehaviour
{
    public CrystalType crystalType;

    private bool isCollecting = false;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollecting) return;

        if (other.gameObject.tag == "Player")
        {
            isCollecting = true;
            other.transform.GetComponent<PlayerController>().CollectCrystalServerRpc(crystalType, other.transform.GetComponent<NetworkObject>().OwnerClientId);
            DestroyServerRpc();
            Destroy(gameObject);
        }   
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        transform.GetComponent<NetworkObject>().Despawn(true);
        Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public enum CrystalType
    {
        Red,
        Green,
        Blue
    }
}
