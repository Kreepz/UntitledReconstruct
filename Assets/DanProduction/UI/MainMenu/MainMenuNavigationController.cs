using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuNavigationController : UiScreenController
{
    [SerializeField] Transform startPos;
    protected override string RootName => "start-menu";
    public override bool ScreenEnabled { get; protected set; }
    
    //menu screens
    VisualElement _startMenu;
    VisualElement _levelBrowser;
    VisualElement _levelDownloader;
    VisualElement _settingsMenu;

    [SerializeField] SimpleInterlop cameraController;

    [Header("Screen contexts")] 
    [SerializeField] MainScreenContext browserContext;
    [SerializeField] MainScreenContext settingsContext;
    
    
    [Header("Screen camera positions")] 
    
    [SerializeField] Transform browserPos;
    [SerializeField] Transform downloaderPos;
    [SerializeField] Transform settingsPos;
    
    //state
    MainMenuStates _currentScreen;

    
    public override void OpenMenu()
    {
        if (ScreenEnabled)
        {
            Debug.LogError("Screen is already enabled");
            return;
        }
        RevealScreen();
        ScreenEnabled = true;
    }

    public override void CloseMenu()
    {
        if (!ScreenEnabled)
        {
            Debug.LogError("Screen is not enabled");
            return;
        }
        HideScreen();
        ScreenEnabled = false;
    }
    
    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        //Queue screens
        _startMenu = Document.rootVisualElement.Q<VisualElement>("start-menu");
        _levelBrowser = Document.rootVisualElement.Q<VisualElement>("level-browser");
        _settingsMenu = Document.rootVisualElement.Q<VisualElement>("settings-menu");
        
        //Prepare visuals
        ClearAllScreens();
        RevealScreen();
        ScreenEnabled = true;
        
        //start menu nav binding
        _startMenu.Q<Button>("browse-level").clicked += () => ChangeMenuScreen(MainMenuStates.LevelBrowser);
        _startMenu.Q<Button>("download-level").clicked += () => ChangeMenuScreen(MainMenuStates.LevelDownloader);
        _startMenu.Q<Button>("settings-button").clicked += () => ChangeMenuScreen(MainMenuStates.Settings);
        
        //level browser nav binding
        _levelBrowser.Q<Button>("return-button").clicked += () => ChangeMenuScreen(MainMenuStates.StartMenu);
        
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
                OpenMenu();
                break;
            case MainMenuStates.LevelBrowser:
            {
                if (browserContext.Controller is LevelBrowserController browser)
                {
                    cameraController.MoveTo(browserContext.CameraPos);
                    browser.OpenMenu(BrowsingContext.Local);
                }
                break;
            }
            case MainMenuStates.LevelDownloader:
            {
                if (browserContext.Controller is LevelBrowserController browser)
                {
                    cameraController.MoveTo(downloaderPos);
                    browser.OpenMenu(BrowsingContext.Online);
                }
                break;
            }
            case MainMenuStates.Settings:
                cameraController.MoveTo(settingsContext.CameraPos);
                settingsContext.Controller.OpenMenu();
                break;
            case MainMenuStates.Close:
                break;
        }
        _currentScreen = newScreen;
    }

    void ClearAllScreens()
    {
        CloseMenu();
        browserContext.Controller.CloseMenu();
        //settingsContext.Controller.CloseMenu();
    }
}
