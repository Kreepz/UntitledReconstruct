using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using SimpleFileBrowser;

public class LevelBrowserController : UiScreenController
{
    //references
    [SerializeField] TaskReportController _reportController;
    [SerializeField] ContentPaginator _paginator;
    [SerializeField] VisualTreeAsset _rowTemplate;
    
    //parameters
    public override bool ScreenEnabled { get; protected set; }
    protected override string RootName => "level-browser";
    
    //state
    BrowsingContext _currentBrowsingContext;
    List<LevelMetadata> _content;
    
    //ui elements
    Label _headerLabel;
    Button _ImportLevelButton;
    ListView _rowList;

    void Start()
    {
        _headerLabel = ScreenRoot.Q<Label>("tab-header");
        _ImportLevelButton = ScreenRoot.Q<Button>("import-content-button");
        _ImportLevelButton.clicked += OnImportPressed;
        _rowList = ScreenRoot.Q<ListView>("rows-list");
        /*
        TemplateContainer rowElementRoot = _rowList.contentContainer.;
        Debug.Log(rowElementRoot == null);
        var color = Color.white;
        color.a = 0;
        rowElementRoot.style.backgroundColor = new StyleColor(color);
        */
    }
    
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
        _content = ctx switch
        {
            BrowsingContext.Local => ContentManager.GetCatalogue(),
            _ => new List<LevelMetadata>()
        };
        LoadCatalogue();
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

    #region Element loading

    void LoadCatalogue()
    {
        _paginator.BuildPages(_content);
        _rowList.itemsSource = _paginator.CurrentPage.Rows;
        _rowList.makeItem = MakeRow;
        _rowList.bindItem = BindRowData;
        
        _rowList.Rebuild();
        Debug.Log("Item source assigned");
    }

    TemplateContainer MakeRow()
    {
        TemplateContainer newRow = _rowTemplate.CloneTree();
        var color = Color.white;
        color.a = 0;
        newRow.style.backgroundColor = new StyleColor(color);
        
        return newRow;
    }
    
    void BindRowData(VisualElement element, int index)
    {
        ContentRow contentRow = element.Q<ContentRow>();

        if (contentRow == null)
        {
            Debug.LogError($"Expected ContentRow, got {element.GetType()}");
            return;
        }
        Debug.Log($"Binding row at index {index}");
        List<LevelMetadata> rowData = _paginator.CurrentPage.Rows[index].Contents;
        contentRow.BindCards(rowData);
    }
    
    #endregion
    
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
    
    void OnFolderSubmitted(string[] paths)
    {
        string packagePath = paths[0];
        TaskResults installResults = ContentManager.InstallLevel(packagePath);
        _reportController.DisplayTaskResult(installResults, 1f, 1f);
    } 
    
    #endregion
    
}

public enum BrowsingContext
{
    Local,
    Online
}