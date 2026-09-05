using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(ExportableLevelRoot))]
public class ExportableLevelRootEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();

        InspectorElement.FillDefaultInspector(root,serializedObject,this);
        
        var button = new Button
        {
            text = "Open Level Compiler"
        };
        button.clicked += CallCompilerWindow;
        root.Add(button);
        return root;
    }

    void CallCompilerWindow()
    {
        if (target is ExportableLevelRoot levelRoot)
        {
            LevelCompilerWindow.PromptWindow(levelRoot);
        }
    }
    /*
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.Space();

        if (GUILayout.Button("Compile level"))
        {
            var levelRoot = (ExportableLevelRoot)target;
            LevelCompilerWindow.ShowWindow(levelRoot);
        }
    }
    */
}
