using System;
using Unity.Collections;
using Unity.Properties;
using UnityEngine;
using UnityEditor;

public class ExportableLevelRoot : MonoBehaviour
{
    [SerializeField, CreateProperty] LevelAuthorMetadata authoredMetadata;
    public LevelAuthorMetadata AuthoredMetadata => authoredMetadata;
    
    public bool ValidateHierarchy()
    {
        bool results = true;
        
        return results;
    }

    public LevelMetadata GetLevelMetadata()
    {
        return new LevelMetadata(authoredMetadata);
    }
}
