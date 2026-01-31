using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    [SerializeField] private HUD hud;
    [SerializeField] private PlayerMovement playerMovement;
    public int duration = 5;
    public int cooldown = 2;
    public bool ignoreRed;
    public bool ignoreGreen;
    public bool ignoreBlue;

    void Start()
    {
        if (GameObject.Find("HUD") != null)
            hud = GameObject.Find("HUD").GetComponent<HUD>();

        hud.abilityDuration = duration;
        hud.abilityCooldown = cooldown;

        ignoreRed = ignoreGreen = ignoreBlue = false;
    }

    void Update()
    {
        // 基础检查：如果完全禁用且白名单为空，直接返回
        if (!playerMovement.enableInput && (playerMovement.inputExceptionKeys == null || playerMovement.inputExceptionKeys.Count == 0))
            return;

        // R 键检查：直接调用 PlayerMovement 里的判断方法
        if (InputController.RedKeyDown)
        {
            if (playerMovement.IsKeyAllowed(KeyCode.R))
            {
                hud.TriggerRed();
            }
        }

        // B 键检查
        if (InputController.BlueKeyDown)
        {
            if (playerMovement.IsKeyAllowed(KeyCode.B))
            {
                hud.TriggerBlue();
            }
        }

        // G 键检查
        if (InputController.GreenKeyDown)
        {
            if (playerMovement.IsKeyAllowed(KeyCode.G))
            {
                hud.TriggerGreen();
            }
        }
    }
}