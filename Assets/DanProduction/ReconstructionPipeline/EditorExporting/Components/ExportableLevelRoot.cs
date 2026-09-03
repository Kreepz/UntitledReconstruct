using System;
using System.IO;
using Unity.Collections;
using Unity.Properties;
using UnityEngine;
using UnityEditor;

public class ExportableLevelRoot : MonoBehaviour
{
    [SerializeField] ExtractableLevelRules extractionRules;
    [SerializeField, CreateProperty] LevelAuthorMetadata authoredMetadata;
    public LevelAuthorMetadata AuthoredMetadata => authoredMetadata;

    public void SetLevelThumbnail(string path)
    {
        authoredMetadata.ThumbnailPath = path;
    }

    public bool IsThumbnailValid()
    {
        return !string.IsNullOrEmpty(authoredMetadata.ThumbnailPath) && File.Exists(authoredMetadata.ThumbnailPath);
    }
    
    public TaskResults ValidateHierarchy()
    {
        return extractionRules.ValidateHierarchy(gameObject);
    }
    
}
