using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.PlayerLoop;

#if UNITY_EDITOR
public class ExportDisplay
{
    string _currentTitle = "";
    string _currentInfo = "";
    float _currentProgress = 0;

    //states
    ExportStage? _currentStage;
    ValidatingStage? _currentValidationTask;
    ResolutionStage? _currentResolutionTask;
    CompilationStage? _currentCompilationTask;
    DeployStage? _currentDeployTask;
    
    public void StartStage(ExportStage newStage)
    {
        if (_currentStage == newStage)
        {
            Debug.LogError($"Trying to double set stage, {newStage}");
            return;
        }
        _currentStage = newStage;
        _currentProgress = 0;
        switch (newStage)
        {
            case ExportStage.Validating:
                _currentTitle = "Validating";
                UpdateTask(ValidatingStage.ValidatingMetadata);
                break;
            case ExportStage.Resolving:
                _currentTitle = "Resolving";
                UpdateTask(ResolutionStage.ResolvingDirectories);
                break;
            case ExportStage.Compiling:
                _currentTitle = "Compiling";
                UpdateTask(CompilationStage.CompilingMetadata);
                break;
            case ExportStage.Deploying:
                _currentTitle = "Deploying";
                UpdateTask(DeployStage.DeployingMetadata);
                break;
            default:
                Debug.LogError($"Invalid stage");
                _currentStage = null;
                break;
        }
    }

    public void UpdateTask(ValidatingStage validationTask)
    {
        if (_currentStage != ExportStage.Validating)
        {
            Debug.LogError($"Current stage is {_currentStage}, attempting to engage in validation task");
            return;
        }
        _currentValidationTask = validationTask;

        _currentInfo = validationTask switch
        {
            ValidatingStage.ValidatingMetadata => "Validating metadata",
            ValidatingStage.ValidatingHierarchy => "Validating level structure",
            _ => "Unknown task"
        };
        
        int taskCount = Enum.GetValues(typeof(ValidatingStage)).Length;
        int currentTask = (int)_currentValidationTask;
        
        _currentProgress = currentTask / (float)taskCount;
        RefreshDisplay();
    }

    public void UpdateTask(ResolutionStage resolutionTask)
    {
        if (_currentStage != ExportStage.Resolving)
        {
            Debug.LogError($"Current stage is {_currentStage}, attempting to engage in resolution task");
            return;
        }
        _currentResolutionTask = resolutionTask;
        
        _currentInfo = resolutionTask switch
        {
            ResolutionStage.ResolvingDirectories => "Resolving export directory",
            ResolutionStage.ResolvingVersion => "Resolving version",
            ResolutionStage.ResolvingImageCompiler => "Resolving image compiler",
            _ => "Unknown task"
        };
        
        int taskCount = Enum.GetValues(typeof(ResolutionStage)).Length;
        int currentTask = (int)_currentResolutionTask;
        
        _currentProgress = currentTask / (float)taskCount;
        RefreshDisplay();
        
    }
    public void UpdateTask(CompilationStage compilationTask)
    {
        if (_currentStage != ExportStage.Compiling)
        {
            Debug.LogError($"Current stage is {_currentStage}, attempting to engage in compilation task");
            return;
        }
        _currentCompilationTask = compilationTask;

        _currentInfo = compilationTask switch
        {
            CompilationStage.CompilingMetadata => "Compiling metadata",
            CompilationStage.CompilingThumbnail => "Compiling thumbnail",
            CompilationStage.CompilingLevel => "Compiling level",
            _ => "Unknown task"
        };
        
        int taskCount = Enum.GetValues(typeof(CompilationStage)).Length;
        int currentTask = (int)_currentCompilationTask;
        
        _currentProgress = currentTask / (float)taskCount;
        RefreshDisplay();
    }

    public void UpdateTask(DeployStage deployTask)
    {
        if (_currentStage != ExportStage.Deploying)
        {
            Debug.LogError($"Current stage is {_currentStage}, attempting to engage in deploy task");
            return;
        }
        _currentDeployTask = deployTask;

        _currentInfo = deployTask switch
        {
            DeployStage.DeployingMetadata => "Deploying metadata",
            DeployStage.DeployingThumbnail => "Deploying thumbnail",
            DeployStage.DeployingLevel => "Deploying level",
            _ => "Unknown task"
        };
        
        int taskCount = Enum.GetValues(typeof(DeployStage)).Length;
        int currentTask = (int)_currentDeployTask;
        
        _currentProgress = currentTask / (float)taskCount;
        RefreshDisplay();
    }
    
    void RefreshDisplay()
    {
        if (string.IsNullOrEmpty(_currentTitle)  || string.IsNullOrEmpty(_currentInfo))
        {
            Debug.LogError($"Invalid progress values," +
                           $"Title : {_currentTitle}, " +
                           $"Info : {_currentInfo}");
            CloseProgressionBar();
            return;
        }
        EditorUtility.DisplayProgressBar(_currentTitle, _currentInfo, _currentProgress);  
    }

    public void CloseProgressionBar()
    {
        if (_currentStage == null)
            Debug.LogError($"Attempting to end progression display, is not currently engaged");
        else if(_currentStage != ExportStage.Deploying)
            Debug.LogWarning($"Ending export display outside of the last stage, current stage : {_currentStage}");
        
        ClearAllValues();
        EditorUtility.ClearProgressBar();
    }

    void ClearAllValues()
    {
        _currentStage = null;
        _currentValidationTask = null;
        _currentResolutionTask = null;
        _currentCompilationTask = null;
        _currentDeployTask = null;
        _currentTitle = "";
        _currentInfo = "";
        _currentProgress = 0;
    }
}

#endif
