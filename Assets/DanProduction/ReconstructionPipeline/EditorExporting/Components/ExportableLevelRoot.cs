using System;
using System.IO;
using Unity.Collections;
using Unity.Properties;
using UnityEngine;
using UnityEditor;

public class ExportableLevelRoot : MonoBehaviour
{
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
        TaskResults results = new();
        results.SubmitResults(true, "Yet to add any rules but working so far!!");
        return results;
    }
    
}
