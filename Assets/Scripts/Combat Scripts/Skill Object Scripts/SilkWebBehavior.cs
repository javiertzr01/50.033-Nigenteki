using Unity.Netcode;
using UnityEngine;

public class SilkWebBehavior : NetworkBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Transform playerTransform;

    [SerializeField] private float maxOpacityDistance = 15f; // Adjust as needed
    [SerializeField] private float opacityChangeSpeed = 1.0f; // Adjust as needed
    [SerializeField] private float fullyOpaqueDistance = 10f; // Adjust as needed

    private float targetOpacity = 0f;
    private float currentOpacity = 0f;

    private void Awake()
    {
        spriteRenderer = transform.parent.gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
    }

    private void UpdateOpacity()
    {
        // Calculate the distance between the SilkWeb and the player
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Calculate the target opacity based on the distance
        float fullyOpaqueRange = maxOpacityDistance - fullyOpaqueDistance;
        targetOpacity = Mathf.Clamp01(1f - Mathf.Clamp01((distanceToPlayer - fullyOpaqueDistance) / fullyOpaqueRange));

        // Gradually change the opacity toward the target
        currentOpacity = Mathf.MoveTowards(currentOpacity, targetOpacity, opacityChangeSpeed * Time.deltaTime);

        // Set the opacity in the material's color
        Color spriteColor = spriteRenderer.material.color;
        spriteColor.a = currentOpacity;
        spriteRenderer.material.color = spriteColor;

        // Enable the sprite when opacity is not zero
        spriteRenderer.enabled = currentOpacity > 0f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerTransform = other.transform;
            UpdateOpacity();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Make the sprite fully transparent when the player exits the trigger zone
            targetOpacity = 0f;
            spriteRenderer.enabled = false;
        }
    }
}
