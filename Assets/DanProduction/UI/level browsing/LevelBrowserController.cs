using System;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using SimpleFileBrowser;

public class LevelBrowserController : UiScreenController
{
    //references
    [SerializeField] TaskReportController _reportController;
    
    //parameters
    public override bool ScreenEnabled { get; protected set; }
    protected override string RootName => "level-browser";
    
    //state
    BrowsingContext _currentBrowsingContext;
    
    //ui elements
    Label _headerLabel;
    Button _ImportLevelButton;

    #region Open/Close functions
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
    

    #endregion
    void Start()
    {
        _headerLabel = ScreenRoot.Q<Label>("tab-header");
        _ImportLevelButton = ScreenRoot.Q<Button>("import-content-button");
        _ImportLevelButton.clicked += OnImportPressed;
    }

    #region Button logic binding

    void OnImportPressed()
    {
        FileBrowser.ShowLoadDialog(
            OnFolderSubmitted,
            ()=> Debug.Log("Import cancelled"),
                SimpleFileBrowser.FileBrowser.PickMode.Folders,
                false,
                null,
                null,
                "Select Level package",
                "Install");
    }
    #endregion
    
    void OnFolderSubmitted(string[] paths)
    {
        string packagePath = paths[0];
        TaskResults installResuts = ContentManager.InstallLevel(packagePath);
        _reportController.DisplayTaskResult(installResuts, 4f, 1f);
    } 
}


public enum BrowsingContext
{
    Local,
    Online
}