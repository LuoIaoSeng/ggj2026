using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class HUD : MonoBehaviour
{
    [SerializeField] private UIDocument hud;
    [SerializeField] private ScreenFilter screenFilter;
    [SerializeField] private PlayerAbility playerAbility;
    Button redButton;
    Button greenButton;
    Button blueButton;
    bool isTriggering = false;
    public int abilityDuration;
    public int abilityCooldown;
    GameObject[] redObjects;
    GameObject[] greenObjects;
    GameObject[] blueObjects;
    void Start()
    {

        redObjects = GameObject.FindGameObjectsWithTag("R");
        // greenObjects = GameObject.FindGameObjectsWithTag("G");
        // blueObjects = GameObject.FindGameObjectsWithTag("B");

        screenFilter = GameObject.Find("ScreenFilter").GetComponent<ScreenFilter>();

        VisualElement root = hud.rootVisualElement;
        redButton = root.Q<Button>("r-button");
        greenButton = root.Q<Button>("g-button");
        blueButton = root.Q<Button>("b-button");

        redButton.clicked += TriggerRed;
        greenButton.clicked += TriggerGreen;
        blueButton.clicked += TriggerBlue;
    }

    private async void Trigger(Button button, Vector3 color, GameObject[] gameObjects, int duration, int cooldown)
    {
        if (button.ClassListContains("active") || button.ClassListContains("cooldown") || isTriggering)
            return;

        foreach (var gameObject in gameObjects)
        {
            var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            var originColor = spriteRenderer.color;
            spriteRenderer.color = new Color(originColor.r, originColor.g, originColor.b, 0.2f);
        }

        isTriggering = true;
        string originalText = button.text;

        button.AddToClassList("active");
        for (int i = duration; i > 0; i--)
        {
            screenFilter.TransitionColor(new Vector4(color.x, color.y, color.z, i / 20.0f), 1);
            await Task.Delay(1000);
        }
        screenFilter.TransitionColor(new Vector4(1, 0, 0, 0), 1);

        foreach (var gameObject in gameObjects)
        {
            var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            var originColor = spriteRenderer.color;
            spriteRenderer.color = new Color(originColor.r, originColor.g, originColor.b, 1);
        }
        button.RemoveFromClassList("active");
        isTriggering = false;

        button.AddToClassList("cooldown");
        for (int i = cooldown; i > 0; i--)
        {
            await Task.Delay(1000);
        }
        button.text = originalText;
        button.RemoveFromClassList("cooldown");
    }

    public async void TriggerRed()
    {
        playerAbility.ignoreRed = true;
        Trigger(redButton, new Vector3(1, 0, 0), redObjects, abilityDuration, abilityCooldown);
        await Task.Delay(abilityDuration * 1000);
        playerAbility.ignoreRed = false;
    }
    public async void TriggerGreen()
    {
        playerAbility.ignoreGreen = true;
        Trigger(greenButton, new Vector3(0, 1, 0), greenObjects, abilityDuration, abilityCooldown);
        await Task.Delay(abilityDuration * 1000);
        playerAbility.ignoreGreen = false;
    }
    public async void TriggerBlue()
    {
        playerAbility.ignoreBlue = true;
        Trigger(blueButton, new Vector3(0, 0, 1), blueObjects, abilityDuration, abilityCooldown);
        await Task.Delay(abilityDuration * 1000);
        playerAbility.ignoreBlue = false;
    }
}