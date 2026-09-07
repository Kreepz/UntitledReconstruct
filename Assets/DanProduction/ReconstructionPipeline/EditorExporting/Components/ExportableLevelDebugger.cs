using System;
using UnityEngine;

public class ExportableLevelDebugger : MonoBehaviour
{
    ReconstructableBehaviour[] _componentsInScene;
    void Awake()
    {
        _componentsInScene = FindObjectsByType<ReconstructableBehaviour>();
        foreach (ReconstructableBehaviour component in _componentsInScene)
            component.OnLevelLoaded();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (ReconstructableBehaviour component in _componentsInScene)
            component.OnLevelStart();
    }
    
}
