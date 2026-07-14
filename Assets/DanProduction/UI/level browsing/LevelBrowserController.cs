using System;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelBrowserController : UiScreenController
{
    //parameters
    public override bool ScreenEnabled { get; protected set; }
    protected override string RootName => "level-browser";
    
    //state
    BrowsingContext _currentBrowsingContext;

    
    
    //ui elements
    Label _headerLabel;
    
    void Start()
    {
        _headerLabel = ScreenRoot.Q<Label>("tab-header");
    }
    
    public void OpenMenu(BrowsingContext ctx)
    {
        _currentBrowsingContext = ctx;

        _headerLabel.text = ctx switch
        {
            BrowsingContext.Local => "Level library",
            BrowsingContext.Online => "Level browser",
            _ => _headerLabel.text
        };
        RevealScreen();
    }
    public override void CloseMenu()
    {
        HideScreen();
    }
    public override void OpenMenu()
    {
        throw new InvalidOperationException(
            "Level browser requires opening context, please use the overloaded function");
    }
}

public enum BrowsingContext
{
    Local,
    Online
}