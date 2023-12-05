using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Profiling;
using UnityEngine;

public class HoneybeeSpray : Projectile
{
    Sprite currentSprite;
    CircleCollider2D coll;
    SpriteRenderer spr;
    public float duration = 2f;
    private float elapsedTime = 0f;
    List<GameObject> enemyInRange = new List<GameObject>();
    List<GameObject> teamInRange = new List<GameObject>();
    private float timer = 0.3f;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
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

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            foreach(GameObject go in enemyInRange)
            {
                ulong sourceClientId = OwnerClientId;
                ulong targetClientId = go.GetComponent<NetworkObject>().OwnerClientId;
                if (go.transform.GetComponent<PlayerController>().playerHealth.Value > 0)
                {
                    go.transform.GetComponent<PlayerController>().TakeDamageServerRpc(Damage, sourceClientId, targetClientId);
                    InstantiateDamageNumberServerRpc(targetClientId);
                    // Can be overwritten
                    ChargeUltimateValue(Damage, 1);
                }
            }

            foreach(GameObject go in teamInRange)
            {
                ulong sourceClientId = OwnerClientId;
                ulong targetClientId = go.GetComponent<NetworkObject>().OwnerClientId;
                if (go.transform.GetComponent<PlayerController>().playerHealth.Value > 0)
                {
                    go.transform.GetComponent<PlayerController>().HealPlayerServerRpc(Damage, targetClientId);
                    // Can be overwritten
                    ChargeUltimateValue(Damage, 1);
                }
            }
        }
    }

    void UpdateCollider()
    {
        float spriteScale = gameObject.GetComponentInChildren<Transform>().localScale.x; // Assuming uniform scaling
        coll.radius = spr.sprite.bounds.extents.x / spriteScale; // Assuming your sprite is a circle
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            // Heal Ally Player Standing Inside
            if (other.transform.GetComponent<PlayerController>().teamId.Value == teamId.Value)
            {
                OnTeamPlayerTriggerStay2D(other);
            }

            // Slowly Damage Enemy Player Standing Inside
            else if (other.transform.GetComponent<PlayerController>().teamId.Value != teamId.Value)
            {
                OnEnemyPlayerTriggerStay2D(other);
            }
        }
        else if (other.gameObject.tag == "Shield")
        {
            OnShieldTriggerStay2D(other);           // Does nothing
        }
        else if (other.gameObject.tag == "Projectile") 
        { 
            OnProjectileTriggerStay2D(other);       // Does nothing
        }
    }

    public override void OnTeamPlayerTriggerEnter2D(Collider2D other)
    {
        base.OnTeamPlayerTriggerEnter2D(other);
        teamInRange.Add(other.gameObject);
    }

    public override void OnTeamPlayerTriggerExit2D(Collider2D other)
    {
        base.OnTeamPlayerTriggerExit2D(other);
        teamInRange.Remove(other.gameObject);
    }

    public override void OnEnemyPlayerTriggerEnter2D(Collider2D other)
    {
        base.OnEnemyPlayerTriggerEnter2D(other);
        enemyInRange.Add(other.gameObject);
    }

    public override void OnEnemyPlayerTriggerExit2D(Collider2D other)
    {
        base.OnEnemyPlayerTriggerExit2D(other);
        enemyInRange.Remove(other.gameObject);
    }

    IEnumerator DamageIfStay()
    {
        yield return new WaitForSeconds(0.3f);
    }
}
