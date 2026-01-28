using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class StartMenuView
{
    public Action OpenSettings
    {
        set
        {
            settingsButton.clicked += value;
        }
    }
    private Button startButton;
    private Button settingsButton;
    private Button quitButton;

    private void Setup()
    {

        quitButton.clicked += () =>
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false; // Stops Play Mode in the Editor
#else
        Application.Quit(); // Quits the built application
#endif
        };
    }

    public StartMenuView(VisualElement root)
    {
        startButton = root.Q<Button>("start-button");
        settingsButton = root.Q<Button>("settings-button");
        quitButton = root.Q<Button>("quit-button");
        
        Setup();
    }
}