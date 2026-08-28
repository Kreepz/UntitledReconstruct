using System;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelPreviewController : UiScreenController
{
    public override bool ScreenEnabled { get; protected set; }
    protected override string RootName => "level-preview";
    
    //references
    LevelMetadata _levelData;
    
    //actions
    Action _onPreviewClose;
    
    //UI elements
    Image _thumbnail;
    Label _levelTitle;
    Label _levelDescription;
    
    void Start()
    {
        _thumbnail = ScreenRoot.Q<Image>("level-thumbnail");
        _levelTitle = ScreenRoot.Q<Label>("level-title");
        _levelDescription = ScreenRoot.Q<Label>("level-description");
    }


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

        _levelData = previewContext.Data;
        _thumbnail.image = ContentManager.GetThumbnail(_levelData);
        _levelTitle.text = _levelData.LevelName;
        _levelDescription.text = _levelData.LevelDescription;

        
        
        RevealScreen();
    }

    public override void CloseMenu()
    {
        _thumbnail.image = null;
        _levelTitle.text = "";
        _levelDescription.text = "";
    }
}

public class PreviewContext
{
    public LevelMetadata Data { get; set; }
    public Action OnClose { get; set; }
}
