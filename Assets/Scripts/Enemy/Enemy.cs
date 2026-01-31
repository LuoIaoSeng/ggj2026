using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected Transform EndPoint1;
    [SerializeField] protected Transform EndPoint2;
    [SerializeField] protected Transform sprite;
    [SerializeField] protected Transform raycastStartPoint;
    protected Camera playerCamera;
    protected PlayerMovement playerMovement;
    protected int direction = 1;
    protected RaycastHit2D hit;
    public bool hitPlayer = false;
    public string mask;
    public bool IsTurn { get; protected set; }
    protected virtual void Start()
    {
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        sprite.position = EndPoint1.position;
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
    protected virtual void Update()
    {
        IsTurn = false;
        if (hit)
        {
            if (hit.collider.tag == "Player")
            {
                var playerAbility = hit.collider.GetComponent<PlayerAbility>();
                if (!isIgnore(playerAbility) && !hitPlayer)
                {
                    HitPlayer();
                }
            }
        }
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
        sprite.DOPlay();
        playerMovement.enableInput = true;
        hitPlayer = false;
    }
    protected async void HitPlayer()
    {
        playerMovement = hit.collider.GetComponent<PlayerMovement>();
        var rb = hit.collider.GetComponent<Rigidbody2D>();

        playerMovement.enableInput = false;
        hitPlayer = true;

        rb.velocity = Vector2.zero;
        playerCamera.DOOrthoSize(5, 1);
        sprite.DOPause();

        await Task.Delay(1000);

        Restart();
    }
    void OnDrawGizmos()
    {
        // Set the color of the gizmo before drawing (optional)
        Gizmos.color = Color.red;

        // Draw the line between the specified start and end points
        Gizmos.DrawLine(raycastStartPoint.position, raycastStartPoint.position + Vector3.right * direction * 10f);
    }
}