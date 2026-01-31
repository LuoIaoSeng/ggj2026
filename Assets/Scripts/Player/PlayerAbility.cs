using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private HUD hud;
    [SerializeField] private PlayerMovement playerMovement;
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

    void Update()
    {
        if (!playerMovement.enableInput)
            return;
        if (InputController.RedKeyDown)
        {
            hud.TriggerRed();
        }
        if (InputController.BlueKeyDown)
        {
            hud.TriggerBlue();
        }
        if (InputController.GreenKeyDown)
        {
            hud.TriggerGreen();
        }
    }
}
