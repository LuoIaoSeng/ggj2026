using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class HUD : MonoBehaviour
{
    [SerializeField] private UIDocument hud;
    [SerializeField] private ScreenFilter screenFilter;
    Button redButton;
    Button greenButton;
    Button blueButton;
    bool isTriggering = false;
    int duration = 5;
    int cooldown = 2;

    void Start()
    {

        screenFilter = GameObject.Find("ScreenFilter").GetComponent<ScreenFilter>();

        VisualElement root = hud.rootVisualElement;
        redButton = root.Q<Button>("r-button");
        greenButton = root.Q<Button>("g-button");
        blueButton = root.Q<Button>("b-button");

        redButton.clicked += TriggerRed;
        greenButton.clicked += TriggerGreen;
        blueButton.clicked += TriggerBlue;
    }

    private async void Trigger(Button button, Vector3 color)
    {
        if (button.ClassListContains("active") || button.ClassListContains("cooldown") || isTriggering)
            return;

        isTriggering = true;
        string originalText = button.text;

        button.AddToClassList("active");
        for (int i = duration; i > 0; i--)
        {
            screenFilter.TransitionColor(new Vector4(color.x, color.y, color.z, i / 20.0f), 1);
            button.text = $"{i}";
            await Task.Delay(1000);
        }
        screenFilter.TransitionColor(new Vector4(1, 0, 0, 0), 1);
        button.RemoveFromClassList("active");
        isTriggering = false;

        button.AddToClassList("cooldown");
        for (int i = cooldown; i > 0; i--)
        {
            button.text = $"{i}";
            await Task.Delay(1000);
        }
        button.text = originalText;
        button.RemoveFromClassList("cooldown");
    }

    public void TriggerRed()
    {
        Trigger(redButton, new Vector3(1, 0, 0));
    }
    public void TriggerGreen()
    {
        Trigger(greenButton, new Vector3(0, 1, 0));
    }
    public void TriggerBlue()
    {
        Trigger(blueButton, new Vector3(0, 0, 1));
    }
}