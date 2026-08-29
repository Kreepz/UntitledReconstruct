using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelPreviewController : UiScreenController
{
    public override bool ScreenEnabled { get; protected set; }
    protected override string RootName => "level-preview";
    
    //references
    [Tooltip("This setup is boilerplate, not much thought or design " +
             "has been put into the author logic and its purely visual")]
    [SerializeField] VisualTreeAsset danoneAuthorPage;
    [SerializeField] VisualTreeAsset katsyAuthorPage;
    
    //relevant data
    LevelMetadata _levelData;
    
    //actions
    Action _onPreviewClose;
    
    //UI elements
    VisualElement _authorContainer;
    Image _thumbnail;
    Label _levelTitle;
    Label _levelDescription;
    DropdownField _versionSelection;
    
    //Buttons
    Button _deleteLevelButton;
    Button _deleteVersionButton;
    Button _returnButton;
    
    void Start()
    {
        _authorContainer = ScreenRoot.Q<VisualElement>("author-panel");
        _thumbnail = ScreenRoot.Q<Image>("level-thumbnail");
        _levelTitle = ScreenRoot.Q<Label>("level-title");
        _levelDescription = ScreenRoot.Q<Label>("level-description");
        
        _versionSelection = ScreenRoot.Q<DropdownField>("version-dropdown");
        _versionSelection.RegisterValueChangedCallback(OnVersionChanged);

        _deleteLevelButton = ScreenRoot.Q<Button>("delete-level-button");
        _deleteVersionButton = ScreenRoot.Q<Button>("delete-version-button");
        
        _returnButton = ScreenRoot.Q<Button>("return-button");
        _returnButton.clicked += CloseMenu;
    }

    #region Loading
    public override void OpenMenu()
    {
        throw new InvalidOperationException(
            "Level preview requires opening context, please use the overloaded function");
    }

    public override void OpenMenuWithContext<T>(T context)
    {
        if (context is not PreviewContext previewContext)
        {
            Debug.LogError("Invalid context passed to the level previewer");
            return;
        }
        LoadElements(previewContext.Data);
        _onPreviewClose = previewContext.OnClose;
        RevealScreen();
    }

    void LoadElements(LevelMetadata data)
    {
        //Load the main display elements
        _levelData = data;
        _thumbnail.image = ContentManager.GetThumbnail(data);
        _levelTitle.text = _levelData.LevelName;
        _levelDescription.text = _levelData.LevelDescription;

        VisualTreeAsset authorCard = data.Author switch
        {
            Authors.danone => danoneAuthorPage,
            Authors.katsy => katsyAuthorPage,
            _ => null
        };
        _authorContainer.Clear();
        _authorContainer.Add(authorCard?.CloneTree());
        
        //Load the setters
        List<int> versions = ContentManager.GetVersions(data);
        List<string> versionOptions = versions.
            Select(version => version.ToString())
            .ToList();
        _versionSelection.choices = versionOptions;
        _versionSelection.value = data.ContentVersion.ToString();
        
        //Load the contextual elements
        if (!data.Official)
        {
            _deleteVersionButton.style.display =
                versions.Count > 1
                    ? DisplayStyle.Flex : DisplayStyle.None;
            _deleteLevelButton.style.display = DisplayStyle.Flex;
        }
        else
        {
            _deleteVersionButton.style.display =  DisplayStyle.None;
            _deleteLevelButton.style.display = DisplayStyle.None;
        }
    }
    #endregion


    #region Logic callbacks
    void OnVersionChanged(ChangeEvent<string> evt)
    {
        if(evt.newValue == _levelData.ContentVersion.ToString())return;
        
        int version = int.Parse(evt.newValue);
        
        LevelMetadata requestedData = ContentManager.GetMetadataVersion(_levelData, version);
        if (requestedData == null) return;
        LoadElements(requestedData);
        
    }
    #endregion
    
    
    public override void CloseMenu()
    {
        _thumbnail.image = null;
        _levelTitle.text = "";
        _levelDescription.text = "";
        _authorContainer.Clear();
        
        
        
        HideScreen();
        _onPreviewClose?.Invoke();
        _onPreviewClose = null;
    }
}

public class PreviewContext
{
    public LevelMetadata Data { get; set; }
    public Action OnClose { get; set; }
}
