using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private HUD hud;
    public int duration = 5;
    public int cooldown = 2;
    void Start()
    {
        hud = GameObject.Find("HUD").GetComponent<HUD>();
        hud.abilityDuration = duration;
        hud.abilityCooldown = cooldown;
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
