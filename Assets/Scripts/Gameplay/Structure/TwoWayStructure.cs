using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoWayStructure : MonoBehaviour
{
    [SerializeField] BoxCollider2D boxCollider2D;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            ToggleCollider(false);
        }
    }
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            ToggleCollider(false);
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            ToggleCollider(true);
        }
    }
    public void ToggleCollider(bool b)
    {
        boxCollider2D.isTrigger = !b;
    }
}
