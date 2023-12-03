using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Crystal : NetworkBehaviour
{
    public CrystalType crystalType;
    public AudioSource crystalAudio;
    public AudioClip collectAudio;

    private bool isCollecting = false;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollecting) return;

        if (other.gameObject.tag == "Player")
        {
            isCollecting = true;
            other.transform.GetComponent<PlayerController>().CollectCrystalServerRpc(crystalType, other.transform.GetComponent<NetworkObject>().OwnerClientId);
            PlayCollectAudioServerRpc(other.transform.GetComponent<NetworkObject>().OwnerClientId);
            StartCoroutine(WaitForSoundBeforeDestroy());
        }   
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayCollectAudioServerRpc(ulong collectedClientId)
    {
        PlayCollectAudioClientRpc(new ClientRpcParams 
        { 
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { collectedClientId }
            }
        });
    }

    [ClientRpc]
    public void PlayCollectAudioClientRpc(ClientRpcParams clientRpcParams = default)
    {
        crystalAudio.PlayOneShot(collectAudio);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        transform.GetComponent<NetworkObject>().Despawn(true);
    }

    public IEnumerator WaitForSoundBeforeDestroy()
    {
        while (crystalAudio.isPlaying)
        {
            transform.GetComponent<SpriteRenderer>().enabled = false;
            yield return null;
        }

        DestroyServerRpc();
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
