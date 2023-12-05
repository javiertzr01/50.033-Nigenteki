using UnityEngine;
using Unity.Netcode;

public class SpawnPointBehaviour : MonoBehaviour
{
    [SerializeField]
    private int teamId; // Set this in the inspector
    [SerializeField]
    private float spawnHealingValue = 50f;

    private SpriteRenderer spriteRenderer;
    [SerializeField] private float visibilityDistance = 10f; // Distance within which the SilkWeb becomes visible to the enemy team
    [SerializeField] private float maxOpacityDistance = 15f; // Distance at which the SilkWeb is fully transparent to the enemy team

    private void Update()
    {
        if (NetworkManager.Singleton.LocalClient.PlayerObject.TryGetComponent(out PlayerController player))
        {
            UpdateVisibility(player);
        }
    }

    private void UpdateVisibility(PlayerController player)
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= visibilityDistance)
        {
            spriteRenderer.enabled = true;
            float opacity = 0.4f - (distanceToPlayer / maxOpacityDistance);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, opacity);
        }
        else
        {
            spriteRenderer.enabled = false;
        }
    }



    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            UpdateVisibility(other.GetComponent<PlayerController>());
        }
        else
        {
            SpawnProtection(other);

        }

    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            // //Logger.Instance.LogInfo("Not running on the server.");
            return;
        }

        PlayerController player = other.gameObject.GetComponent<PlayerController>();

        if (player != null)
        {
            if (player.teamId.Value == this.teamId)
            {
                // //Logger.Instance.LogInfo($"Healing player {player.gameObject.name}");
                player.HealPlayerServerRpc(spawnHealingValue * Time.deltaTime, player.GetComponent<NetworkObject>().OwnerClientId);
            }
            else
            {
                // //Logger.Instance.LogInfo($"Player {player.gameObject.name} is from a different team.");
            }
        }

        SpawnProtection(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            UpdateVisibility(other.GetComponent<PlayerController>());
        }
    }


    private void SpawnProtection(Collider2D other)
    {
        Projectile projectile = other.gameObject.GetComponent<Projectile>();
        SkillObject skillObject = other.gameObject.GetComponent<SkillObject>();

        if (projectile != null && projectile.teamId.Value != this.teamId)
        {
            //Logger.Instance.LogInfo("Destroy Projectile in Spawn");
            projectile.DestroyServerRpc();
        }
        else if (skillObject != null && skillObject.teamId.Value != this.teamId)
        {
            skillObject.DestroyServerRpc();
        }
    }
}
