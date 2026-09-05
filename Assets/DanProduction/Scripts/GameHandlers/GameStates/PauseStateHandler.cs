using System;
using UnityEngine;
using UnityEngine.UIElements;

public class PauseStateHandler : MonoBehaviour
{
    //UI references
    UIDocument _uiDocument;
    Label _levelTitle;
    Label _levelDescription;
    Label _levelAuthor;
    Label _levelVersion;
    Image _levelThumbnail;
    
    Button _resumeButton;
    Button _restartButton;
    Button _mainMenuButton;

    void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
        _uiDocument.rootVisualElement.style.display = DisplayStyle.None;
        
        _levelTitle = _uiDocument.rootVisualElement.Q<Label>("level-title");
        _levelDescription = _uiDocument.rootVisualElement.Q<Label>("level-desc");
        _levelAuthor = _uiDocument.rootVisualElement.Q<Label>("level-author");
        _levelVersion = _uiDocument.rootVisualElement.Q<Label>("level-version");
        _levelThumbnail = _uiDocument.rootVisualElement.Q<Image>("level-thumbnail");
        
        _resumeButton = _uiDocument.rootVisualElement.Q<Button>("resume-button");
        _resumeButton.clicked += () => GameStateManager.SetState(GameStates.Running);

        _restartButton = _uiDocument.rootVisualElement.Q<Button>("restart-level-button");
        _restartButton.clicked += () => GameStateManager.SetState(GameStates.ReloadingLevel);
        
        _mainMenuButton = _uiDocument.rootVisualElement.Q<Button>("main-menu-button");
        _mainMenuButton.clicked += GameSessionManager.ReturnToMainMenu;
        
        GameStateManager.SetupState(GameStates.Paused, OnGamePause, OnGameResume);
    }

    void Start()
    {
        if(GameSessionManager.CurrentLevel != null)
            PopulateUI(GameSessionManager.CurrentLevel);
    }

    void PopulateUI(LevelMetadata level)
    {
        _levelTitle.text = level.LevelName;
        _levelDescription.text = level.LevelDescription;
        _levelAuthor.text = $"By {level.Author.ToString()}";
        _levelVersion.text = $"Version - {level.ContentVersion}";
        _levelThumbnail.image = ContentManager.GetThumbnail(level);
    }

    void OnGamePause()
    {
        _uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
    }

    void OnGameResume()
    {
        _uiDocument.rootVisualElement.style.display = DisplayStyle.None;
    }
}
