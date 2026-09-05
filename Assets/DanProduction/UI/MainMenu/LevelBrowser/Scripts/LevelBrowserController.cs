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
    [SerializeField] LevelPreviewController previewController;
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
    
    //ui elements
    Label _headerLabel;
    Label _currentPageLabel;
    
    Button _importLevelButton;
    ListView _rowList;
    
    Button _nextPageButton;
    Button _previousPageButton;

    void Start()
    {
        //Labels
        _headerLabel = ScreenRoot.Q<Label>("tab-header");
        _currentPageLabel = ScreenRoot.Q<Label>("page-number-label");
        
        //button binding
        _importLevelButton = ScreenRoot.Q<Button>("import-content-button");
        _importLevelButton.clicked += OnImportPressed;
        
        _nextPageButton = ScreenRoot.Q<Button>("next-button");
        _nextPageButton.clicked += NavigateNextPage;
        
        _previousPageButton = ScreenRoot.Q<Button>("previous-button");
        _previousPageButton.clicked += NavigatePreviousPage;
        
        //initialise row list
        _rowList = ScreenRoot.Q<ListView>("rows-list");
        _rowList.makeItem = MakeRow;
        _rowList.bindItem = BindRowData;
    }
    
    #region Open/Close functions
    public override void OpenMenuWithContext<T>(T openingContext)
    {
        if (openingContext is not BrowsingContext ctx)
        {
            Debug.LogError("Browser opened without appropriate context");
            return;
        }
        
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

    void SoftLoadCatalogue()
    {
        LoadCatalogue(false);
    }
    
    void LoadCatalogue(bool resetPage = true)
    {
        PaginationContext paginationCtx = new PaginationContext()
        {
            Source = _sourceContent(),
            ThumbnailResolver = _resolveThumbnail,
            OnCardInteract = PreviewLevel,
            ResetPage = resetPage
        };
        
        paginator.BuildPages(paginationCtx);
        
        RefreshCatalogueUI();
    }
    
    void RefreshCatalogueUI()
    {
        if (paginator.CurrentPage == null)
        {
            _rowList.itemsSource = null;
            _rowList.Rebuild();

            _currentPageLabel.text = "";
            _nextPageButton.style.display = DisplayStyle.None;
            _previousPageButton.style.display = DisplayStyle.None;
            return;
        }
        
        _rowList.itemsSource = paginator.CurrentPage.Rows;
        _rowList.Rebuild();
        
        _currentPageLabel.text = paginator.CurrentPageNumber.ToString();
        _nextPageButton.style.display = paginator.HasNextPage ? DisplayStyle.Flex : DisplayStyle.None;
        _previousPageButton.style.display = paginator.HasPreviousPage ? DisplayStyle.Flex : DisplayStyle.None;
    }
    
    TemplateContainer MakeRow()
    {
        TemplateContainer newRow = rowTemplate.CloneTree();
        
        //Overwrite the style assigned by the list element
        var color = Color.white;
        color.a = 0;
        newRow.style.backgroundColor = new StyleColor(color);
        
        //Initialise the classes
        ContentRow rowClass = newRow.Q<ContentRow>();
        rowClass.InitialiseElement();
        
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
    //Page navigation
    void NavigateNextPage()
    {
        if (!paginator.HasNextPage) return;
        paginator.NextPage();
        RefreshCatalogueUI();
    }

    void NavigatePreviousPage()
    {
        if (!paginator.HasPreviousPage) return;
        paginator.PreviousPage();
        RefreshCatalogueUI();
    }
    
    //Importing
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
        if(installResults.Success) LoadCatalogue();
    }

    void PreviewLevel(LevelMetadata metadata)
    {
        HideScreen();

        PreviewContext previewCtx = new()
        {
            Data =  metadata,
            OnClose = RevealScreen,
            OnDataModified = SoftLoadCatalogue
        };
        previewController.OpenMenuWithContext(previewCtx);
    }
    #endregion
}

public enum BrowsingContext
{
    Local,
    Online
}