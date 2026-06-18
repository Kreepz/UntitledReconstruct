using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class MainMenuUiController : MonoBehaviour
{
     UIDocument _document;
     VisualElement _root;
     
     //elements
     Button _selectLevelButton;
     UnityEvent OnSelectLevelButtonPressed;
     
     private void Awake()
     {
          if(TryGetComponent(out _document))
          {
              _root = _document.rootVisualElement;
              _selectLevelButton = _root.Q<Button>("select-level");
              _selectLevelButton.clicked += OnSelectLevelButtonPressed.Invoke;
          }
     }

     void OnButtonClicked()
     {
         Debug.Log("OnButtonClicked");
     }
}
