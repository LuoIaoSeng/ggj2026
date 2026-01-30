using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lift : MonoBehaviour
{
    void OnTriggerStay2D(Collider2D collision)
    {
        if(InputController.Interact)
        {
            if(collision.tag == "Player")
            {
                var playerMovement = collision.GetComponent<PlayerMovement>();
                playerMovement.enableInput = false;
            }
        }
    }
}
