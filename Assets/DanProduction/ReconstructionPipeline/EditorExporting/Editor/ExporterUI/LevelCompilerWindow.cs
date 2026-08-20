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
    
    //input elements
    ObjectField _thumbnailField;
    VisualElement _versioningField;
    Toggle _automaticVersionToggle;
    
    //button elements
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
        
        //style inclusion
        if(compilerStyle)rootVisualElement.styleSheets.Add(compilerStyle);
    }
    
    void BindData()
    {
        //default metadata editors
       _levelRoot.AuthoredMetadata.GenerateNewId();
       rootVisualElement.dataSource = _levelRoot.AuthoredMetadata;
       
       //custom metadata fields
       _thumbnailField = rootVisualElement.Q<ObjectField>("thumbnail-field");
       _thumbnailField.value = _levelRoot.AuthoredMetadata.Thumbnail;
       
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
        _thumbnailField.RegisterValueChangedCallback(OnThumbnailChanged);
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
}
