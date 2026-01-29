using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private HUD hud;
    void Start()
    {
        hud = GameObject.Find("HUD").GetComponent<HUD>();
    }

    void Update()
    {
        if(InputController.RedKeyDown)
        {
            hud.TriggerRed();
        }
        if(InputController.BlueKeyDown)
        {
            hud.TriggerBlue();
        }
        if(InputController.GreenKeyDown)
        {
            hud.TriggerGreen();
        }
    }
}
