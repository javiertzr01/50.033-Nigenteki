using Unity.Netcode;
using UnityEngine;

public class SilkWebBehavior : NetworkBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] private float visibilityDistance = 10f; // Distance within which the SilkWeb becomes visible to the enemy team
    [SerializeField] private float maxOpacityDistance = 15f; // Distance at which the SilkWeb is fully transparent to the enemy team


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
    }

    private void Update()
    {
        if (NetworkManager.Singleton.LocalClient.PlayerObject.TryGetComponent(out PlayerController player))
        {
            UpdateVisibility(player);
        }
    }

    private void UpdateVisibility(PlayerController player)
    {
        int silkWebTeamId = transform.parent.GetComponent<SilkWeb>().teamId.Value; // Assuming SilkWeb has a NetworkVariable<int> teamId
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (player.teamId.Value == silkWebTeamId)
        {
            // If the player is on the same team as the SilkWeb, it's always fully visible
            spriteRenderer.enabled = true;
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
        }
        else
        {
            // If the player is on a different team, adjust the SilkWeb's opacity based on distance
            if (distanceToPlayer <= visibilityDistance)
            {
                spriteRenderer.enabled = true;
                float opacity = 1f - (distanceToPlayer / maxOpacityDistance);
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, opacity);
            }
            else
            {
                spriteRenderer.enabled = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            UpdateVisibility(other.GetComponent<PlayerController>());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            UpdateVisibility(other.GetComponent<PlayerController>());
        }
    }
}
