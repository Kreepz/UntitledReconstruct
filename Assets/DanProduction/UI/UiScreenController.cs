using System;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class UiScreenController : MonoBehaviour
{
    protected UIDocument Document;
    protected VisualElement ScreenRoot;
    public abstract bool ScreenEnabled { get; protected set; }
    protected abstract string RootName { get; }
    protected virtual void Awake()
    {
        Document = GetComponentInParent<UIDocument>();
        if (!Document)
        {
            Debug.LogError("No document found in parent");
            return;
        }
        ScreenRoot = Document.rootVisualElement.Q<VisualElement>(RootName);
        if (ScreenRoot == null)
        {
            Debug.LogError("No screen root found");
            return;
        }
    }

    protected void RevealScreen()
    {
        ScreenRoot.style.display = DisplayStyle.Flex;
    }
    protected void HideScreen()
    {
        ScreenRoot.style.display = DisplayStyle.None;
    }
    
    public abstract void OpenMenu();
    public abstract void CloseMenu();
}
