using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StartMenuPresenter : MonoBehaviour
{

    [SerializeField] private UIDocument presenter;
    private VisualElement root;
    private VisualElement startMenuView;
    private VisualElement settingsView;
    
    void ToggleSettings(bool b)
    {
        startMenuView.SetDisplay(!b);
        settingsView.SetDisplay(b);
    }

    void Start()
    {
        presenter = GetComponent<UIDocument>();
        root = presenter.rootVisualElement;
        startMenuView = root.Q("StartMenuView");
        settingsView = root.Q("SettingsView");

        StartMenuView startMenuViewPresenter = new StartMenuView(startMenuView);
        SettingsView settingsViewPresenter = new SettingsView(settingsView);

        startMenuViewPresenter.OpenSettings = () =>
        {
            ToggleSettings(true);
        };

        settingsViewPresenter.CloseSettings = () =>
        {
            ToggleSettings(false);
        };
    }
}
