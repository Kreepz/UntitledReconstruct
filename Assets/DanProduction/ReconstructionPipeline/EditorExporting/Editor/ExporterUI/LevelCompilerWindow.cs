using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelCompilerWindow : EditorWindow
{
    //data references
    [SerializeField] VisualTreeAsset compilerWindow;
    [SerializeField] StyleSheet compilerStyle;
    [SerializeField] Texture2D defaultThumbnail;
    
    public ExportSettings _settings;
    
    
    //Generic caches
    VisualElement _root;
    ExportableLevelRoot _levelRoot;
    
    //UI elements
    VisualElement _versioningField;
    Image _thumbnailPreview;
    Toggle _automaticVersionToggle;
    
    //button elements
    Button _selectThumbnailButton;
    Button _exportButton;
    Button _shipButton;
    Button _exportShipButton;
    
    
    #region Initialisation
    public static void PromptWindow(ExportableLevelRoot level)
    {
        var window = GetWindow<LevelCompilerWindow>();
        
        window.titleContent = new GUIContent("Level Compiler");
        window.minSize = new Vector2(400, 450);

        window._settings = new ExportSettings();
        window._levelRoot = level;
        window._settings.ImportPreferences();
        window._settings.DefaultThumbnail = window.defaultThumbnail;
        window.CreateWindow();
        window.BindData();
        window.BindBehaviour();
    }
    
    void CreateWindow()
    {
        //document initialisation
        rootVisualElement.Clear();
        _root = compilerWindow.CloneTree();
        var root = _root.Q<VisualElement>("root");
        rootVisualElement.Add(root ?? _root);
        Undo.undoRedoPerformed += OnUndo;
        
        //style inclusion
        if(compilerStyle)rootVisualElement.styleSheets.Add(compilerStyle);
    }
    
    void BindData()
    {
        //default metadata editors
       _levelRoot.AuthoredMetadata.GenerateNewId();
       rootVisualElement.dataSource = _levelRoot.AuthoredMetadata;
       
       //other UI elements
       _thumbnailPreview = rootVisualElement.Q<Image>("level-thumbnail");
       
       //export window settings
       BindExportSettings();
    }
    
    void BindBehaviour()
    {
        _exportButton =  rootVisualElement.Q<Button>("export-button");
        if(_exportButton == null) 
        { 
            Debug.LogError("Export button not found");
            return;
        }
        _exportButton.clicked += ExportLevel;

        _shipButton = rootVisualElement.Q<Button>("ship-button");
        if (_shipButton == null)
        {
            Debug.LogError("Ship button not found");
            return;
        }
        _shipButton.clicked += ShipLevel;
        
        _selectThumbnailButton = rootVisualElement.Q<Button>("thumbnail-selection-button");
        if (_selectThumbnailButton == null)
        {
            Debug.LogError("Thumbnail button not found");
            return;
        }
        _selectThumbnailButton.clicked += SelectThumbnail;
        
        UpdateThumbnailPreview();
        //register callbacks after initial setup
        rootVisualElement.schedule.Execute(RegisterCallbacks).StartingIn(100);
    }
    
    void RegisterCallbacks()
    {
        RegisterDirtyCallback(rootVisualElement.Q<TextField>("level-name-field"));
        RegisterDirtyCallback(rootVisualElement.Q<TextField>("level-description-field"));
        RegisterDirtyCallback(rootVisualElement.Q<EnumField>("author-field"));
        RegisterDirtyCallback(rootVisualElement.Q<IntegerField>("manual-version-field"));
        RegisterDirtyCallback(rootVisualElement.Q<FloatField>("required-version-field"));
    }
    
    void BindExportSettings()
    {
        _automaticVersionToggle = rootVisualElement.Q<Toggle>("version-automation-field");
        _automaticVersionToggle.value = _settings.AutomaticVersioning;
        _versioningField = rootVisualElement.Q("manual-version-field");
        _versioningField.SetEnabled(!_settings.AutomaticVersioning);
        _versioningField.style.display = !_settings.AutomaticVersioning ? DisplayStyle.Flex : DisplayStyle.None;
        
        _automaticVersionToggle.RegisterValueChangedCallback(evt =>
        {
            ToggleAutoVersioning(evt.newValue);
        });
    }
    
    #endregion

    #region Button functions

    void ExportLevel()
    {
        string exportPath = EditorUtility.OpenFolderPanel(
            "Choose Export Location",
            "",
            "");

        if (string.IsNullOrEmpty(exportPath)) return;
        LocalExporter.ExportLevel(_levelRoot, _settings, new DirectoryInfo(exportPath));
    }
    void ShipLevel()
    {
        LocalExporter.ExportLevel(_levelRoot, _settings, LocalPaths.ShipExport);
    }

    void SelectThumbnail()
    {
        string thumbnailPath = EditorUtility.OpenFilePanel(
            "Choose thumbnail",
            "",
            "png");
        if (string.IsNullOrEmpty(thumbnailPath)) return;
        if (!File.Exists(thumbnailPath)) return;
        _levelRoot.SetLevelThumbnail(thumbnailPath);
        UpdateThumbnailPreview();
        _thumbnailPreview.image = _levelRoot.AuthoredMetadata.GetPreviewThumbnail();
    }
    
    #endregion
    
    #region Callback functions
    void ToggleAutoVersioning(bool toggle)
    {
        LevelCompilerPreferences.AutoVersion = toggle;
        _settings.AutomaticVersioning = toggle;
        _versioningField.style.display = !toggle ? DisplayStyle.Flex : DisplayStyle.None;
        _versioningField.SetEnabled(!toggle);
    }
    
    #endregion
    
    #region Helper functions

    void OnUndo()
    {
        UpdateThumbnailPreview();
        EditorUtility.SetDirty(_levelRoot);
    }
    
    void RegisterDirtyCallback<T>(BaseField<T> field)
    {
        if (field == null)
        {
            Debug.LogError("Trying to register callback on a null field");
            return;
        }
        field.RegisterValueChangedCallback(evt =>
        {
            if (!EqualityComparer<T>.Default.Equals(evt.previousValue, evt.newValue))
            {
                Undo.RecordObject(_levelRoot, "Edit Level Metadata");
                EditorUtility.SetDirty(_levelRoot);
            }
        });
    }
    
    void OnThumbnailChanged(ChangeEvent<Object> evt)
    {
        var thumbnail = evt.newValue is Texture2D newTexture ? newTexture : defaultThumbnail;
        _levelRoot.AuthoredMetadata.Thumbnail = thumbnail;

        if (evt.previousValue == evt.newValue) return;
        Undo.RecordObject(_levelRoot, "Edit Level Metadata");
        EditorUtility.SetDirty(_levelRoot);
    }
    #endregion

    void UpdateThumbnailPreview()
    {
        string path = _levelRoot.AuthoredMetadata.ThumbnailPath;

        _selectThumbnailButton.text =
            string.IsNullOrEmpty(path) ? "None" : path;
        
        //otherwise attempt to load
        Texture2D preview = _levelRoot.AuthoredMetadata.GetPreviewThumbnail();

        if (preview)
        {
            _thumbnailPreview.image = preview;
            _selectThumbnailButton.RemoveFromClassList("invalid");
        }

        else
        {
            _thumbnailPreview.image = defaultThumbnail;
            _selectThumbnailButton.AddToClassList("invalid");
        }
    }
    
}
