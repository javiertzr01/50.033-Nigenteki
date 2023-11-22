using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

public class HoneybeeSpray : Projectile
{
    Sprite currentSprite;
    CircleCollider2D coll;
    SpriteRenderer spr;
    public float duration = 2f;
    private float elapsedTime = 0f;
    protected override void Start()
    {
        base.Start();
        coll = gameObject.GetComponent<CircleCollider2D>();
        spr = gameObject.GetComponentInChildren<SpriteRenderer>();
        UpdateCollider();

    }

    protected override void Update()
    {
        base.Update();
        if (currentSprite != spr.sprite)
        {
            currentSprite = spr.sprite;
            UpdateCollider();
        }

        // Gradually increase size and decrease opacity
        if (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float scaleFactor = Mathf.Lerp(1f, 2f, t); // Adjust the scale factor as needed
            float opacity = Mathf.Lerp(0.7f, 0f, t);     // Adjust the opacity as needed

            // Access the transform property from the GameObject
            transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
            spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, opacity);

            elapsedTime += Time.deltaTime;
        }
    }

    void UpdateCollider()
    {
        float spriteScale = gameObject.GetComponentInChildren<Transform>().localScale.x; // Assuming uniform scaling
        coll.radius = spr.sprite.bounds.extents.x / spriteScale; // Assuming your sprite is a circle
    }

    public override void CollisionEnter2DLogic(Collision2D collision)
    {
        DestroyServerRpc();
    }

    public override void TriggerEnter2DLogic(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            instantiatingArm.ChargeUltimate(Damage, 30);
            // Friendly Player here
            // TODO: Implement a list within the PlayerController that stores the unique healing abilities on the player

        }
    }

    public override void TriggerStay2DLogic(Collider2D other)
    {
        // If friendly player
        // Check if the HoneybeeSpray is within the HealStore list, if not then heal player

        // If enemy player
        // Slowly damage enemy players who stand inside with a damage value of 1
    }

    public override void TriggerExit2DLogic(Collider2D other)
    {
        // Remove "Honeybee Spray" from Player HealStore list
    }

}
