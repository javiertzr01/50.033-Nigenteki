using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public interface IAudioHelper
{

    void PlayAudioForAllClients(int audioTypeIndex, ClientRpcParams clientRpcParams = default);
    void SetAudioSourceClips(int audioTypeIndex, out AudioSource audioSource, out AudioClip audioClip);
    [ServerRpc(RequireOwnership = false)]
    void CastAudioSFXServerRpc(int audioTypeIndex, ServerRpcParams serverRpcParams = default);
    [ClientRpc]
    void PlayAudioClientRpc(int audioTypeIndex, Vector3 otherPlayerPosition, ClientRpcParams clientRpcParams = default);
    void PlayAudio(int audioTypeIndex, Vector3 otherPlayerPosition);
}
