using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected Transform EndPoint1;
    [SerializeField] protected Transform EndPoint2;
    [SerializeField] protected Transform enemyObject;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    protected Camera playerCamera;
    protected PlayerMovement playerMovement;
    protected int direction = 1;
    public bool hitPlayer = false;
    public string mask;
    public bool IsTurn { get; protected set; }
    protected virtual void Start()
    {
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        // enemyObject.position = EndPoint1.position;
    }
    protected bool isIgnore(PlayerAbility playerAbility)
    {
        switch (mask)
        {
            case "red":
                return playerAbility.ignoreRed;
            case "blue":
                return playerAbility.ignoreBlue;
            case "green":
                return playerAbility.ignoreGreen;
        }
        return false;
    }
    protected void Restart()
    {
        var checkpoints = FindObjectsOfType<Checkpoint>();
        foreach (var checkpoint in checkpoints)
        {
            if (checkpoint.checkpointIndex == GameInstance.Instance.gameData.checkpoint)
            {
                playerCamera.DOOrthoSize(15, 1).WaitForCompletion();
                playerMovement.transform.DOMove(checkpoint.transform.position, 1);
                break;
            }
        }
        enemyObject.DOPlay();
        playerMovement.enableInput = true;
        hitPlayer = false;
    }

    protected void OnTriggerEnter2D(Collider2D collision)
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

        if (enemyObject != null) enemyObject.DOPause();

        await Task.Delay(1000);

        Restart();
    }
}