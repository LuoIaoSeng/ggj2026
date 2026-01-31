using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Lift : MonoBehaviour
{
    [SerializeField] private List<Transform> floorTransforms;
    [SerializeField] private int floor;
    [SerializeField] private string dir;
    [SerializeField] private int initFloor;
    [SerializeField] private Transform lift;
    private Collider2D player;
    private bool isMoving = false;

    private ItemRequirement itemRequirement;

    void Start()
    {
        floor = initFloor;
        itemRequirement = GetComponent<ItemRequirement>();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            player = collision;
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        player = null;
    }
    void Update()
    {
        if (InputController.Interact && player && !isMoving)
        {
            if (itemRequirement != null && !itemRequirement.CheckAccess(player.gameObject))
            {
                // 这里可以加一些反馈，比如播放“嘟嘟”的报错音效
                return;
            }

            var playerMovement = player.GetComponent<PlayerMovement>();
            var rb = playerMovement.GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.zero;
            playerMovement.enableInput = false;
            isMoving = true;

            var temp = floor + (dir == "up" ? 1 : -1);
            if (temp == floorTransforms.Count)
            {
                dir = "down";
            }
            else if (temp == -1)
            {
                dir = "up";
            }
            floor = floor + (dir == "up" ? 1 : -1);
            playerMovement.transform.DOMoveX(floorTransforms[floor].position.x, 0.5f)
            .OnComplete(() =>
            {
                playerMovement.transform.DOMoveY(floorTransforms[floor].position.y, 2);
                lift.transform.DOMoveY(floorTransforms[floor].position.y, 2).OnComplete(() =>
                {
                    playerMovement.enableInput = true;
                    isMoving = false;
                });
            });
        }
    }
}
