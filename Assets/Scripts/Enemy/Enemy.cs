using DG.Tweening;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform EndPoint1;
    [SerializeField] private Transform EndPoint2;
    [SerializeField] private Transform sprite;
    [SerializeField] private Transform raycastStartPoint;
    private int direction = 1;
    public bool hitPlayer = false;
    public string mask;
    void Start()
    {
        sprite.position = EndPoint1.position;
        sprite
            .DOMove(EndPoint2.position, 2)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Yoyo).OnStepComplete(() =>
            {
                sprite.DORotate(new Vector3(0, sprite.rotation.eulerAngles.y + 180, 0), 0);
                direction *= -1;
            });
    }
    bool isIgnore(PlayerAbility playerAbility)
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
    void Update()
    {
        var hit = Physics2D.Raycast(raycastStartPoint.position, Vector3.right * direction, 10f);
        if (hit)
        {
            if (hit.collider.tag == "Player")
            {
                var playerAbility = hit.collider.GetComponent<PlayerAbility>();
                if (!isIgnore(playerAbility) && !hitPlayer)
                {
                    var playerMovement = hit.collider.GetComponent<PlayerMovement>();
                    var rb = hit.collider.GetComponent<Rigidbody2D>();
                    playerMovement.enableInput = false;
                    rb.velocity = Vector2.zero;
                    var camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
                    camera.DOOrthoSize(5, 1);
                    sprite.DOPause();
                    hitPlayer = true;
                }
            }
        }
    }
    void OnDrawGizmos()
    {
        // Set the color of the gizmo before drawing (optional)
        Gizmos.color = Color.red;

        // Draw the line between the specified start and end points
        Gizmos.DrawLine(raycastStartPoint.position, raycastStartPoint.position + Vector3.right * direction * 10f);
    }
}