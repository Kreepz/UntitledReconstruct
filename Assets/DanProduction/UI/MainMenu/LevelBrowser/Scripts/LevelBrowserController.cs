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
    [SerializeField] TaskReportController reportController;
    [SerializeField] ContentPaginator paginator;
    [SerializeField] VisualTreeAsset rowTemplate;
    
    //parameters
    public override bool ScreenEnabled { get; protected set; }
    protected override string RootName => "level-browser";
    
    //handlers
    Func<List<LevelMetadata>> _sourceContent;
    Func<LevelMetadata, Texture2D> _resolveThumbnail;
    
    //state
    BrowsingContext _currentBrowsingContext;
    List<LevelMetadata> _content;
    
    //ui elements
    Label _headerLabel;
    Button _importLevelButton;
    ListView _rowList;

    void Start()
    {
        _headerLabel = ScreenRoot.Q<Label>("tab-header");
        
        //button binding
        _importLevelButton = ScreenRoot.Q<Button>("import-content-button");
        _importLevelButton.clicked += OnImportPressed;
        
        //initialise row list
        _rowList = ScreenRoot.Q<ListView>("rows-list");
        _rowList.makeItem = MakeRow;
        _rowList.bindItem = BindRowData;
    }
    
    #region Open/Close functions
    public void OpenMenu(BrowsingContext ctx)
    {
        _currentBrowsingContext = ctx;
        switch (_currentBrowsingContext)
        {
            case BrowsingContext.Local:
                _headerLabel.text = "Level library";
                _sourceContent = ContentManager.GetCatalogue;
                _resolveThumbnail = ContentManager.GetThumbnail;
                break;
            case BrowsingContext.Online:
                _headerLabel.text = "Level browser";
                Debug.LogWarning("Remote importer not yet implemented");
                break;
        }
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
        paginator.BuildPages(_sourceContent(), _resolveThumbnail);
        _rowList.itemsSource = paginator.CurrentPage.Rows;
        _rowList.Rebuild();
    }
    
    TemplateContainer MakeRow()
    {
        TemplateContainer newRow = rowTemplate.CloneTree();
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
        List<ContentCardData> rowData = paginator.CurrentPage.Rows[index].Contents;
        contentRow.BindCards(rowData);
    }
    
    #endregion
    
    #region Button logic binding

    void OnImportPressed()
    {
        FileBrowser.ShowLoadDialog(
            OnFolderSubmitted,
            ()=> Debug.Log("Import cancelled"),
                FileBrowser.PickMode.Folders,
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
        reportController.DisplayTaskResult(installResults, 1f, 1f);
        if(installResults.Success) RebuildCatalogue();
    }

    void RebuildCatalogue()
    {
        _content = _currentBrowsingContext switch
        {
            BrowsingContext.Local => ContentManager.GetCatalogue(),
            _ => null
        };
        LoadCatalogue();
    }
    #endregion
}

public enum BrowsingContext
{
    Local,
    Online
}