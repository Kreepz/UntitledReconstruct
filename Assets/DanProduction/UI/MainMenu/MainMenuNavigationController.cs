using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuNavigationController : MonoBehaviour
{
    UIDocument _document;
    
    //menu screens
    VisualElement _startMenu;
    VisualElement _levelBrowser;
    VisualElement _levelDownloader;
    VisualElement _settingsMenu;

    [SerializeField] SimpleInterlop cameraController;
    
    [Header("Screen camera positions")] 
    [SerializeField] Transform startPos;
    [SerializeField] Transform browserPos;
    [SerializeField] Transform downloaderPos;
    [SerializeField] Transform settingsPos;
    
    //state
    MainMenuStates _currentScreen;
    
    void Awake()
    {
        if (!TryGetComponent(out _document)) return;
        
        //Queue screens
        _startMenu = _document.rootVisualElement.Q<VisualElement>("start-menu");
        _levelBrowser = _document.rootVisualElement.Q<VisualElement>("level-browser");
        _levelDownloader = _document.rootVisualElement.Q<VisualElement>("level-downloader");
        _settingsMenu = _document.rootVisualElement.Q<VisualElement>("settings-menu");
        
        //Prepare visuals
        ClearAllScreens();
        _startMenu.style.display = DisplayStyle.Flex;
        _currentScreen = MainMenuStates.StartMenu;
        
        //start menu nav binding
        _startMenu.Q<Button>("browse-level").clicked += () => ChangeMenuScreen(MainMenuStates.LevelBrowser);
        _startMenu.Q<Button>("download-level").clicked += () => ChangeMenuScreen(MainMenuStates.LevelDownloader);
        _startMenu.Q<Button>("settings-button").clicked += () => ChangeMenuScreen(MainMenuStates.Settings);
        
        //level browser nav binding
        _levelBrowser.Q<Button>("return-button").clicked += () => ChangeMenuScreen(MainMenuStates.StartMenu);
        
        //online level browser nav binding
        _levelDownloader.Q<Button>("return-button").clicked += () => ChangeMenuScreen(MainMenuStates.StartMenu);
        
        //settings menu nav binding
        _settingsMenu.Q<Button>("return-button").clicked += () => ChangeMenuScreen(MainMenuStates.StartMenu);
    }
    
    void ChangeMenuScreen(MainMenuStates newScreen)
    {
        if (newScreen == _currentScreen) return;
        ClearAllScreens();

        switch (newScreen)
        {
            case MainMenuStates.StartMenu:
                cameraController.MoveTo(startPos);
                _startMenu.style.display = DisplayStyle.Flex;
                break;
            case MainMenuStates.LevelBrowser:
                cameraController.MoveTo(browserPos);
                _levelBrowser.style.display = DisplayStyle.Flex;
                break;
            case MainMenuStates.LevelDownloader:
                cameraController.MoveTo(downloaderPos);
                _levelDownloader.style.display = DisplayStyle.Flex;
                break;
            case MainMenuStates.Settings:
                cameraController.MoveTo(settingsPos);
                _settingsMenu.style.display = DisplayStyle.Flex;
                break;
            case MainMenuStates.Close:
                break;
        }
        _currentScreen = newScreen;
    }

    void ClearAllScreens()
    {
        _startMenu.style.display = DisplayStyle.None;
        _levelBrowser.style.display = DisplayStyle.None;
        _levelDownloader.style.display = DisplayStyle.None;
        _settingsMenu.style.display = DisplayStyle.None;
    }
}
