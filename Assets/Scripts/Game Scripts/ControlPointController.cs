using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ControlPointController : NetworkBehaviour, IAudioHelper
{
    public GameStateStore gameStateStore;
    public NetworkStore netStore;
    private ControlPointTeamOccupancyState currentOccupancyState;

    private AudioSource audioSource0;
    private AudioSource audioSource1;
    private AudioSource audioSource2;

    public AudioClip audioSFX0;   // Assign this in the Inspector
    public AudioClip audioSFX1;   // Assign this in the Inspector
    public AudioClip audioSFX2;   // Assign this in the Inspector

    enum ControlPointTeamOccupancyState
    {
        NoPlayers,
        BothTeamsPresent,
        OnlyTeam1Present,
        OnlyTeam2Present
    }

    private void Awake()
    {
        audioSource0 = gameObject.AddComponent<AudioSource>();
        audioSource1 = gameObject.AddComponent<AudioSource>();
        audioSource2 = gameObject.AddComponent<AudioSource>();
    }


    // Start is called before the first frame update
    void Start()
    {
        ControlPointPositionServerRpc();
        gameStateStore.isControlPointActive.Value = false;
        gameStateStore.currentTeamOnControlPoint.Value = 0;
        currentOccupancyState = ControlPointTeamOccupancyState.NoPlayers;
    }

    // Update is called once per frame
    void Update()
    {
        EvaluateControlPointOccupancy();
        UpdateControlPointStatusServerRpc();

        if (audioSource0 != null && audioSFX0 != null && gameStateStore.isControlPointActiveReducingTimer.Value && !audioSource0.isPlaying)
        {
            CastAudioSFXServerRpc(0);
        }

    }

    [ServerRpc(RequireOwnership = false)]
    public void ControlPointPositionServerRpc(ServerRpcParams serverRpcParams = default)
    {
        transform.position = new Vector3(netStore.generatedMapData.Value.CapturePointPosition[0], netStore.generatedMapData.Value.CapturePointPosition[1], 0);
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateControlPointStatusServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        if (OwnerClientId != clientId) return;

        UpdateControlPointStatus();
    }

    private void UpdateControlPointStatus()
    {
        switch (currentOccupancyState)
        {
            case ControlPointTeamOccupancyState.OnlyTeam1Present:
                gameStateStore.isControlPointActive.Value = true;
                gameStateStore.currentTeamOnControlPoint.Value = 1;
                
                break;

            case ControlPointTeamOccupancyState.OnlyTeam2Present:
                gameStateStore.isControlPointActive.Value = true;
                gameStateStore.currentTeamOnControlPoint.Value = 2;

                break;

            case ControlPointTeamOccupancyState.NoPlayers:
            case ControlPointTeamOccupancyState.BothTeamsPresent:
            default:
                gameStateStore.isControlPointActive.Value = false;
                gameStateStore.currentTeamOnControlPoint.Value = 0;
                if (audioSource0 != null && audioSource0.isPlaying)
                {
                    //StopCastAudioSFXServerRpc(0);
                }
                break;
        }
    }

    private void EvaluateControlPointOccupancy()
    {
        if (gameStateStore.numberOfTeam1PlayersOnControlPoint.Value <= 0 && gameStateStore.numberOfTeam2PlayersOnControlPoint.Value <= 0)
        {
            currentOccupancyState = ControlPointTeamOccupancyState.NoPlayers;
        }
        else if (gameStateStore.numberOfTeam1PlayersOnControlPoint.Value > 0 && gameStateStore.numberOfTeam2PlayersOnControlPoint.Value > 0)
        {
            currentOccupancyState = ControlPointTeamOccupancyState.BothTeamsPresent;
        }
        else if (gameStateStore.numberOfTeam1PlayersOnControlPoint.Value > 0)
        {
            currentOccupancyState = ControlPointTeamOccupancyState.OnlyTeam1Present;
        }
        else // gameStateStore.numberOfTeam2PlayersOnControlPoint.Value > 0
        {
            currentOccupancyState = ControlPointTeamOccupancyState.OnlyTeam2Present;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnTriggerEnter2DServerRpc(collision.transform.GetComponent<PlayerController>().teamId.Value, collision.transform.GetComponent<PlayerController>().OwnerClientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnTriggerEnter2DServerRpc(int teamId, ulong clientId, ServerRpcParams serverRpcParams = default)
    {

        if (teamId == 0)
        {
            //CHECKS FOR DUPLICATE
            if (!gameStateStore.team1PlayersIDOnControlPoint.Contains(clientId))
            {
                gameStateStore.team1PlayersIDOnControlPoint.Add(clientId);
            }
            gameStateStore.numberOfTeam1PlayersOnControlPoint.Value = gameStateStore.team1PlayersIDOnControlPoint.Count;
        }
        else if (teamId == 1)
        {
            if (!gameStateStore.team2PlayersIDOnControlPoint.Contains(clientId))
            {
                gameStateStore.team2PlayersIDOnControlPoint.Add(clientId);
            }
            gameStateStore.numberOfTeam2PlayersOnControlPoint.Value = gameStateStore.team2PlayersIDOnControlPoint.Count;
        }

    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnTriggerExit2DServerRpc(collision.transform.GetComponent<PlayerController>().teamId.Value, collision.transform.GetComponent<PlayerController>().OwnerClientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnTriggerExit2DServerRpc(int teamId, ulong clientId, ServerRpcParams serverRpcParams = default)
    {
        if (teamId == 0)
        {
            if (gameStateStore.team1PlayersIDOnControlPoint.Contains(clientId))
            {
                gameStateStore.team1PlayersIDOnControlPoint.Remove(clientId);
            }
            gameStateStore.numberOfTeam1PlayersOnControlPoint.Value = gameStateStore.team1PlayersIDOnControlPoint.Count;
        }
        else if (teamId == 1)
        {
            if (gameStateStore.team2PlayersIDOnControlPoint.Contains(clientId))
            {
                gameStateStore.team2PlayersIDOnControlPoint.Remove(clientId);
            }
            gameStateStore.numberOfTeam2PlayersOnControlPoint.Value = gameStateStore.team2PlayersIDOnControlPoint.Count;
        }

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
}
