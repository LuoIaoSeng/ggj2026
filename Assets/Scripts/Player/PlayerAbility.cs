using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private HUD hud;
    public int duration = 5;
    public int cooldown = 2;
    public bool ignoreRed;
    public bool ignoreGreen;
    public bool ignoreBlue;
    void Start()
    {
        hud = GameObject.Find("HUD").GetComponent<HUD>();
        hud.abilityDuration = duration;
        hud.abilityCooldown = cooldown;

        ignoreRed = ignoreGreen = ignoreBlue = false;
    }

    async void Update()
    {
        if(InputController.RedKeyDown)
        {
            hud.TriggerRed();
            ignoreRed = true;
            await Task.Delay(duration * 1000);
            ignoreRed = false;
        }
        if(InputController.BlueKeyDown)
        {
            hud.TriggerBlue();
            ignoreBlue = true;
            await Task.Delay(duration * 1000);
            ignoreBlue = false;
        }
        if(InputController.GreenKeyDown)
        {
            hud.TriggerGreen();
            ignoreGreen = true;
            await Task.Delay(duration * 1000);
            ignoreGreen = false;
        }
    }
}
