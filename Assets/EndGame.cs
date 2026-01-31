using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    [SerializeField] private ItemRequirement itemRequirement;
    [SerializeField] private ScreenFilter screenFilter;
    async void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision)
        {
            if (collision.CompareTag("Player") && itemRequirement.CheckAccess(collision.gameObject))
            {
                var playerMovemnet = collision.gameObject.GetComponent<PlayerMovement>();
                playerMovemnet.enableInput = false;
                screenFilter.TransitionColor(new Vector4(0, 0, 0, 1), 1);
                await Task.Delay(1000);
                SceneManager.LoadScene("EndScene");
            }
        }
    }
}
