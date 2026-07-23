using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelCompilerWindow : EditorWindow
{
    //data references
    [SerializeField] VisualTreeAsset compilerWindow;
    [SerializeField] StyleSheet compilerStyle;
    
    
    //caches
    VisualElement _root;
    ExportableLevelRoot _levelRoot;
    [SerializeField] Texture2D defaultThumbnail;
    ObjectField _thumbnailField;

    #region Initialisation
    public static void PromptWindow(ExportableLevelRoot level)
    {
        var window = GetWindow<LevelCompilerWindow>();
        
        window.titleContent = new GUIContent("Level Compiler");
        window.minSize = new Vector2(400, 450);
        
        
        window._levelRoot = level;
        window.CreateWindow();
        window.BindData();
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
       _levelRoot.AuthoredMetadata.GenerateNewId();
       rootVisualElement.dataSource = _levelRoot.AuthoredMetadata;
       
       //setup custom field (thumbnail)
       _thumbnailField = rootVisualElement.Q<ObjectField>("thumbnail-field");
       _thumbnailField.value = _levelRoot.AuthoredMetadata.Thumbnail;
       
       //register callbacks after initial binding
       rootVisualElement.schedule.Execute(RegisterCallbacks).StartingIn(100);
    }

    void RegisterCallbacks()
    {
        RegisterDirtyCallback(rootVisualElement.Q<TextField>("level-name-field"));
        RegisterDirtyCallback(rootVisualElement.Q<TextField>("level-description-field"));
        RegisterDirtyCallback(rootVisualElement.Q<EnumField>("author-field"));
        RegisterDirtyCallback(rootVisualElement.Q<FloatField>("version-field"));
        RegisterDirtyCallback(rootVisualElement.Q<FloatField>("required-version-field"));
        _thumbnailField.RegisterValueChangedCallback(OnThumbnailChanged);
        Debug.Log("Callbacks registered");
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
