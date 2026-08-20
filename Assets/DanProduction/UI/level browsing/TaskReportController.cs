using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class TaskReportController : MonoBehaviour
{
    [SerializeField] Color successColour = Color.green;
    [SerializeField] Color failureColour = Color.red;
    
    Coroutine _fadeCoroutine;
    
    //ui and elements
    UIDocument _document;
    VisualElement _rootContainer;
    VisualElement _resultContainer;
    Label _resultCaption;
    Label _resultWarning;
    Label _resultError;
    Button _closeButton;
    
    void Start()
    {
        //setting up references
        _document = GetComponentInParent<UIDocument>();
        if (!_document)
        {
            Debug.LogError("No document found in parent");
            return;
        }
        _rootContainer = _document.rootVisualElement.Q<VisualElement>("overlay");
        _resultContainer = _document.rootVisualElement.Q<VisualElement>("task-result-panel");
        if (_resultContainer == null)
        {
            Debug.LogError("No root container found");
            return;
        }
        _closeButton = _resultContainer.Q<Button>("close-button");
        _resultCaption = _resultContainer.Q<Label>("result-caption");
        _resultWarning = _resultContainer.Q<Label>("warning-list");
        _resultError = _resultContainer.Q<Label>("error-list");
        
        
        //logic binding
        _closeButton.clicked += Conceal;
    }

    public void DisplayTaskResult(TaskResults results, float displayDuration, float fadeDuration)
    {
        Debug.Log("Producing results");
        if (!results.ResultSubmitted)
        {
            Debug.LogError("The result attempted to display has not been submitted");
            return;
        }
        //reset visual state
        _rootContainer.style.display = DisplayStyle.Flex;
        _resultContainer.style.opacity = 1f;
        
        //reset labels
        _resultCaption.text = "";
        _resultWarning.text = "";
        _resultError.text = "";
        
        //set border colour
        Color borderColour = results.Success ? successColour : failureColour;
        _resultContainer.style.borderTopColor = borderColour;
        _resultContainer.style.borderBottomColor = borderColour;
        _resultContainer.style.borderLeftColor = borderColour;
        _resultContainer.style.borderRightColor = borderColour;
        
        //populate labels
        _resultCaption.text = results.Caption;
        if (results.Warnings.Count > 0)
        {
            string warningList = $"Warnings: {string.Join("\n", results.Warnings)}";
            _resultWarning.text = warningList;
        }
        if (results.Errors.Count > 0)
        {
            string errorList = $"Errors: {string.Join("\n", results.Errors)}";
            _resultError.text = errorList;
        }

        if (results.Success)
        {
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
                _fadeCoroutine = null;
            }
            _fadeCoroutine = StartCoroutine(FadeOut(displayDuration, fadeDuration));
        }
        else
        {
            _closeButton.style.display = DisplayStyle.Flex;
        }
    }

    IEnumerator FadeOut(float displayDuration, float fadeDuration)
    {
        yield return new WaitForSeconds(displayDuration);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            
            float opacity = 1f - (elapsed / fadeDuration);
            _resultContainer.style.opacity = opacity;
            yield return null;
        }
        _resultContainer.style.opacity = 0f;
        Conceal();
    }
    
    void Conceal()
    {
        _rootContainer.style.display = DisplayStyle.None;
        _closeButton.style.display = DisplayStyle.None;
        if (_fadeCoroutine != null) 
        {
            StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = null;
        }
    }
}
