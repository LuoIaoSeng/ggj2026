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
    private GameData gameData;
    private SliderInt globalVolumnSilder;
    private SliderInt gameVolumnSilder;
    private SliderInt musicVolumnSilder;

    private void Setup()
    {
        applyButton.clicked += () =>
        {
            gameData.globalVolumn = globalVolumnSilder.value;
            gameData.gameVolumn = gameVolumnSilder.value;
            gameData.musicVolumn = musicVolumnSilder.value;
            GameInstance.Instance.gameData = gameData;
            GameInstance.Instance.SaveData();
            GameInstance.Instance.UpdateSettings();
        };
    }

    public SettingsView(VisualElement root)
    {
        GameInstance.Instance.LoadData();
        gameData = GameInstance.Instance.gameData;
        var dropdown = root.Q<DropdownField>("resolution-dropdown");
        globalVolumnSilder = root.Q<SliderInt>("global-volumn-slider");
        gameVolumnSilder = root.Q<SliderInt>("game-volumn-slider");
        musicVolumnSilder = root.Q<SliderInt>("music-volumn-slider");
        foreach (var str in gameData.resolutions)
        {
            dropdown.choices.Add(str);
        }
        cancelButton = root.Q<Button>("cancel-button");
        applyButton = root.Q<Button>("apply-button");

        var fullscreen = root.Q<Toggle>("fullscreen-toggle");
        fullscreen.RegisterCallback<ChangeEvent<bool>>((evt) =>
        {
            gameData.fullscreen = evt.newValue;
        });
        dropdown.RegisterValueChangedCallback(evt =>
        {
            gameData.resolution = evt.newValue;
        });

        Setup();
    }
}