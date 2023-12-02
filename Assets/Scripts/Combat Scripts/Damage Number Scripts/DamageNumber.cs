using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class DamageNumber : NetworkBehaviour
{
    private float fadeOutTime = 0.3f;
    private float startTime;
    public NetworkVariable<float> damage = new NetworkVariable<float>();
    private Vector3 startPos;
    private Color originalColor;
    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        startTime = Time.time;
        startPos = transform.position;
        originalColor = transform.GetComponent<TextMeshPro>().color;
    }

    // Update is called once per frame
    void Update()
    {
        transform.GetComponent<TextMeshPro>().text = "-" + damage.Value.ToString();
        TextFadeClientRpc();
        DestroyAfterFadeOutServerRpc();
    }

    public void Initialize()
    {
        transform.position += new Vector3(Random.Range(-0.5f, 0.5f), 0.5f, 0);
    }

    [ClientRpc]
    public void TextFadeClientRpc(ClientRpcParams clientRpcParams = default)
    {
        transform.GetComponent<TextMeshPro>().color = Color.Lerp(originalColor, Color.clear, EasingFade());

        transform.position = Vector3.Lerp(startPos, startPos + Vector3.up * 0.2f, EasingFade());
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyAfterFadeOutServerRpc()
    {
        if (!IsServer) return;

        if (Time.time < startTime + fadeOutTime) return;

        transform.GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }

    public float EasingFade()
    {
        float currentTime = Time.time;
        return Mathf.Pow(((currentTime - startTime) / fadeOutTime), 4);
    }
}
