using UnityEngine;
using Unity.Netcode;

public class AudioHelper : NetworkBehaviour, IAudioHelper
{
    private AudioSource audioSource0;
    private AudioSource audioSource1;
    private AudioSource audioSource2;

    public AudioClip audioSFX0;   // Assign this in the Inspector
    public AudioClip audioSFX1;   // Assign this in the Inspector
    public AudioClip audioSFX2;   // Assign this in the Inspector


    private void Awake()
    {
        audioSource0 = gameObject.AddComponent<AudioSource>();
        audioSource1 = gameObject.AddComponent<AudioSource>();
        audioSource2 = gameObject.AddComponent<AudioSource>();
    }

    public void PlayAudioForAllClients(int audioTypeIndex, ClientRpcParams clientRpcParams = default)
    {
        AudioSource audioSource;
        AudioClip audioClip;
        SetAudioSourceClips(audioTypeIndex, out audioSource, out audioClip);

        if (audioClip != null)
        {
            foreach (var playerClientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                Vector3 otherPlayerPosition = NetworkManager.Singleton.ConnectedClients[playerClientId].PlayerObject.transform.position;

                PlayAudioClientRpc(audioTypeIndex, otherPlayerPosition, new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[] { playerClientId }
                    }
                });
            }
        }
    }

    public virtual void SetAudioSourceClips(int audioTypeIndex, out AudioSource audioSource, out AudioClip audioClip)
    {
        switch (audioTypeIndex)
        {
            case 2:
                audioSource = audioSource2;
                audioClip = audioSFX2;
                break;
            case 1:
                audioSource = audioSource1;
                audioClip = audioSFX1;
                break;
            default:
            case 0:
                audioSource = audioSource0;
                audioClip = audioSFX0;
                break;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void CastAudioSFXServerRpc(int audioTypeIndex, ServerRpcParams serverRpcParams = default)
    {
        PlayAudioForAllClients(audioTypeIndex);
    }

    [ClientRpc]
    public void PlayAudioClientRpc(int audioTypeIndex, Vector3 otherPlayerPosition, ClientRpcParams clientRpcParams = default)
    {
        PlayAudio(audioTypeIndex, otherPlayerPosition);
    }

    public void PlayAudio(int audioTypeIndex, Vector3 otherPlayerPosition)
    {
        AudioSource audioSource;
        AudioClip audioClip;
        SetAudioSourceClips(audioTypeIndex, out audioSource, out audioClip);

        if (audioClip != null && audioSource != null)
        {
            // Calculate the distance between this player and the other player
            Vector2 relativePosition = otherPlayerPosition - transform.position;

            float maxPanDistance = 5f;
            float panExponent = 2f;     // A quadratic curve for more pronounced panning
            float volumeExponent = 3f;  // A quadratic curve for more pronounced volume

            // Define the maximum distance at which the sound can be heard
            float maxDistance = 100f;

            // Exponential stereo pan based on the horizontal position (left or right)
            float panStereo = -Mathf.Sign(relativePosition.x) * Mathf.Pow(Mathf.Clamp(Mathf.Abs(relativePosition.x) / maxPanDistance, 0f, 1f), panExponent);
            audioSource.panStereo = panStereo;

            // Adjust volume exponentially based on distance
            float distance = Vector2.Distance(transform.position, otherPlayerPosition);

            float volumeRatio = Mathf.Clamp(1 - (distance / maxDistance), 0, 1);
            float volume = Mathf.Pow(volumeRatio, volumeExponent);

            audioSource.PlayOneShot(audioClip, volume);
        }
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
