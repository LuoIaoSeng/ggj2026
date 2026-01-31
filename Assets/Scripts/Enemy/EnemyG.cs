using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class EnemyG : Enemy
{
    protected override void Start()
    {
        base.Start();
        mask = "green"; 
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollision(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleCollision(collision.gameObject);
    }

    private void HandleCollision(GameObject obj)
    {
        if (hitPlayer) return;

        if (obj.CompareTag("Player"))
        {
            var ability = obj.GetComponent<PlayerAbility>();

            if (ability != null && isIgnore(ability))
            {
                return;
            }

            KillPlayer(obj);
        }
    }

    private async void KillPlayer(GameObject playerObj)
    {
        playerMovement = playerObj.GetComponent<PlayerMovement>();
        var rb = playerObj.GetComponent<Rigidbody2D>();

        hitPlayer = true;

        if (playerMovement != null) playerMovement.enableInput = false;
        if (rb != null) rb.velocity = Vector2.zero;

        if (playerCamera != null) playerCamera.DOOrthoSize(5, 1);

        if (sprite != null) sprite.DOPause();

        await Task.Delay(1000);

        Restart();
    }
}