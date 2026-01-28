using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsView
{
    private Button cancelButton;

    private void Setup()
    {

        // cancelButton.clicked;
    }

    public SettingsView(VisualElement root)
    {
        cancelButton = root.Q<Button>("cancel-button");
        
        Setup();
    }
}