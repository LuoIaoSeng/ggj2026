using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsView
{
    public Action CloseSettings
    {
        set
        {
            cancelButton.clicked += value;
            applyButton.clicked += value;
        }
    }
    private Button cancelButton;
    private Button applyButton;

    private void Setup()
    {

        applyButton.clicked += () =>
        {
            
        };
    }

    public SettingsView(VisualElement root)
    {
        cancelButton = root.Q<Button>("cancel-button");
        applyButton = root.Q<Button>("apply-button");
        
        Setup();
    }
}