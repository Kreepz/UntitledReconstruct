using System;
using UnityEngine;
using UnityEngine.UIElements;

public class LoadStateHandler : MonoBehaviour
{
    //UI references
    UIDocument _uiDocument;
    Label _levelTitle;
    Label _authorSubtitle;
    Image _thumbnailImage;
    Label _currentAction;
    
    Button _mainMenuButton;
    Button _playButton;
    
    //Services
    LevelReconstructor _levelConstructor = new();
    
    
    void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
        _uiDocument.rootVisualElement.style.display = DisplayStyle.None;
        
        _levelTitle = _uiDocument.rootVisualElement.Q<Label>("level-title");
        _authorSubtitle = _uiDocument.rootVisualElement.Q<Label>("level-creator");
        _thumbnailImage = _uiDocument.rootVisualElement.Q<Image>("level-profile-image");
        _currentAction = _uiDocument.rootVisualElement.Q<Label>("current-action");
        
        _mainMenuButton = _uiDocument.rootVisualElement.Q<Button>("exit-button");
        _mainMenuButton.clicked += GameSessionManager.ReturnToMainMenu;
        
        _playButton = _uiDocument.rootVisualElement.Q<Button>("play-button");
        _playButton.clicked += StartGame;
            //() => GameStateManager.SetState(GameStates.InitialisingLevel);
        
        GameStateManager.SetupState(GameStates.LoadingLevel, OnGameLoad);
        GameStateManager.SetupState(GameStates.ReloadingLevel, OnGameReload);
    }

    void Start()
    {
        if(GameSessionManager.CurrentLevel != null)
            PopulateUI(GameSessionManager.CurrentLevel);
    }

    void OnDestroy()
    {
        GameStateManager.ClearState(GameStates.LoadingLevel);
    }
    
    
    async void OnGameLoad()
    {
        LevelMetadata level = GameSessionManager.CurrentLevel;
        if (level == null)
        {
            Debug.LogError("No level set");
            return;
        }
        _uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
        _playButton.style.display = DisplayStyle.None;
        _currentAction.text = "Loading level...";

        TaskResults buildResults = await _levelConstructor.ReconstructLevel(level);
        if (buildResults.Success)
        {
            _levelConstructor.LoadBehaviourComponents();
            _currentAction.text = "";
            _playButton.style.display = DisplayStyle.Flex;
        }
        else
        {
            Debug.LogError("Failed to construct level");
        }
    }

    async void OnGameReload()
    {
        _uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
        _currentAction.text = "Reloading level...";
        
        TaskResults reloadResults =  await _levelConstructor.ReloadLevel();
        if (reloadResults.Success)
        {
            _levelConstructor.LoadBehaviourComponents();
            _currentAction.text = "";
            _playButton.style.display = DisplayStyle.Flex;
        }
        else
        {
            Debug.LogError("Failed to load level");
        }
    }

    void StartGame()
    {
        _levelConstructor.StartBehaviourComponents();
        _uiDocument.rootVisualElement.style.display = DisplayStyle.None;
        GameStateManager.SetState(GameStates.InitialisingLevel);
    }
    
    void PopulateUI(LevelMetadata level)
    {
        _levelTitle.text = level.LevelName;
        _authorSubtitle.text = level.Author.ToString();
        _thumbnailImage.image = ContentManager.GetThumbnail(level);
    }
}
