using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSwitcher : MonoBehaviour
{
    private ScreenFilter screenFilter;
    [SerializeField] private string level;
    void Start()
    {
        screenFilter = GameObject.Find("ScreenFilter").GetComponent<ScreenFilter>();
    }
    async void OnTriggerEnter2D(Collider2D collision)
    {
        var playerMovement = collision.GetComponent<PlayerMovement>();
        playerMovement.enableInput = false;
        screenFilter.TransitionColor(new Vector4(0, 0, 0, 1), 1);
        await Task.Delay(1000);
        SceneManager.LoadScene(level);
    }
}
